using Laboratory_Service.Application.Interface;
using Laboratory_Service.Infrastructure.Data;
using Laboratory_Service.Infrastructure.Repositories;
using Laboratory_Service.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddGrpcClient<IAM_Service.API.Protos.UserService.UserServiceClient>(options =>
            {
                var grpcUrl = configuration["IAMService:GrpcUrl"] ?? "http://localhost:7001";
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
            // --- 2. Repository Registrations ---
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddScoped<ITestOrderRepository, TestOrderRepository>();
            services.AddScoped<IPatientService,PatientService>();
            services.AddScoped<IIamUserService, IamUserGrpcClientService>();
            services.AddScoped<IEventLogService, EventLogRepository>();
            services.AddScoped<IEncryptionService, DeterministicAesEncryptionService>();
            return services;
        }
    }
}
