using Infrastructure.Repositories;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.Services;
using Laboratory_Service.Infrastructure.Data;
using Laboratory_Service.Infrastructure.GrpcClient;
using Laboratory_Service.Infrastructure.GrpcClients;
using Laboratory_Service.Infrastructure.RabbitMQ;
using Laboratory_Service.Infrastructure.Repositories;
using Laboratory_Service.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Simulator.API.Protos.Query;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Laboratory_Service.Infrastructure
{
    public static class InfrastructureDI
    {
        /// <summary>
        /// Registers all infrastructure-level services such as DbContext, repositories, and authentication providers.
        /// Called from Program.cs in the API layer.
        /// </summary>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            // --- 1. Database Context Registration ---
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                // Ưu tiên lấy từ biến môi trường DATABASE_URL (Render, Railway, v.v.)
                var envConnection = Environment.GetEnvironmentVariable("DATABASE_URL");

                // Nếu không có thì lấy trong appsettings.json
                var connectionString = !string.IsNullOrEmpty(envConnection)
                    ? ConvertPostgresUrlToConnectionString(envConnection)
                    : config.GetConnectionString("DefaultConnection");

                // Lấy schema từ appsettings.json, mặc định là "laboratory_service"
                var schemaName = config["Database:Schema"] ?? "laboratory_service";
                
                // Set schema vào static property của AppDbContext
                Data.AppDbContext.SchemaName = schemaName;

                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    // Cấu hình migration history table vào schema riêng
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", schemaName);
                });
            });

            services.AddGrpcClient<IAM_Service.API.Protos.UserService.UserServiceClient>(options =>
            {
                var grpcUrl = configuration["IAMService:GrpcUrl"] ?? "http://localhost:7001";
                options.Address = new Uri(grpcUrl);
            }).ConfigureChannel(options =>
            {
                var grpcUrl = configuration["IAMService:GrpcUrl"] ?? "http://localhost:7001";
                var isHttps = !string.IsNullOrEmpty(grpcUrl) && grpcUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
                
                var httpHandler = new System.Net.Http.SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    ConnectTimeout = TimeSpan.FromSeconds(30) // Connection timeout
                };

                // On Render production with HTTPS, trust server certificate
                // Render load balancer handles SSL termination
                // gRPC over HTTPS requires proper certificate validation
                if (isHttps)
                {
                    httpHandler.SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                    {
                        RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                    };
                }

                options.HttpHandler = httpHandler;
                // Set max send/receive message size
                options.MaxReceiveMessageSize = 6 * 1024 * 1024; // 6 MB
                options.MaxSendMessageSize = 6 * 1024 * 1024;    // 6 MB
            });

            services.AddGrpcClient<RawDataQueryService.RawDataQueryServiceClient>(options =>
            {
                var grpcUrl = configuration["SimulatorService:GrpcUrl"] ?? "http://localhost:7003";
                options.Address = new Uri(grpcUrl);
            }).ConfigureChannel(options =>
            {
                options.HttpHandler = new System.Net.Http.SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan
                };
            });
            services.AddHostedService<RabbitMQRawResultConsumer>();
            // --- 2. Repository Registrations ---
            services.AddScoped<IRawBackupRepository, RawBackupRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddScoped<ITestOrderRepository, TestOrderRepository>();
            services.AddScoped<ITestResultRepository, TestResultRepository>();
            services.AddScoped<IFlaggingConfigRepository, FlaggingConfigRepository>();
            services.AddScoped<IProcessedMessageRepository, ProcessedMessageRepository>();
            services.AddScoped<IPatientService,PatientService>();


            services.AddScoped<IIamUserService, IamUserGrpcClientService>();
            services.AddScoped<ISimulatorGrpcClient, SimulatorGrpcClient>();
            services.AddScoped<IEventLogService, EventLogRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IEncryptionService, DeterministicAesEncryptionService>();
            services.AddSingleton<IAiReviewService, AiReviewService>();

            return services;
        }
        
        private static string ConvertPostgresUrlToConnectionString(string url)
        {
            // URL kiểu: postgresql://user:pass@host:port/db hoặc postgresql://user:pass@host/db
            var uri = new Uri(url);
            var userInfo = uri.UserInfo.Split(':');
            var username = userInfo[0];
            var password = userInfo.Length > 1 ? userInfo[1] : "";

            // Nếu không có port trong URL, dùng port mặc định 5432 cho PostgreSQL
            var port = uri.Port == -1 ? 5432 : uri.Port;

            return $"Host={uri.Host};Port={port};Database={uri.AbsolutePath.TrimStart('/')};Username={username};Password={password};Ssl Mode=Require;Trust Server Certificate=true";
        }
    }
}
