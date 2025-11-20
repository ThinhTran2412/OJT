using Laboratory_Service.Application;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.Services;
using Laboratory_Service.Infrastructure;
using Laboratory_Service.Infrastructure.Services;
using Microsoft.OpenApi.Models;
using MyCompany.Authorization.Setup;
using SharedLibrary.DependencyInjection;

namespace Laboratory_Service.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create a builder for the web application.
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Laboratory Service API",
                    Version = "v1",
                    Description = "API for managing patients and medical records in Laboratory Service"
                });
            });

            // Add gRPC services
            builder.Services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaxReceiveMessageSize = 6291456;
                options.MaxSendMessageSize = 6291456;
            });

            // Configure Kestrel
            if (builder.Environment.IsProduction())
            {
                // Production: Use PORT from environment variable (Render default)
                builder.WebHost.ConfigureKestrel(options =>
                {
                    var port = Environment.GetEnvironmentVariable("PORT");
                    var portNumber = !string.IsNullOrEmpty(port) ? int.Parse(port) : 8080;
                    options.ListenAnyIP(portNumber, listenOptions =>
                    {
                        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
                    });
                });
            }
            else
            {
                // Development: Use localhost with specific port
                builder.WebHost.ConfigureKestrel(options =>
                {
                    // Setup a HTTP/2 endpoint without TLS.
                    options.ListenLocalhost(7156, o => o.Protocols =
                        Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
                });
            }

            builder.Services.AddSingleton<ITokenService, IAMTokenService>();

            builder.Services.AddHttpClient<IIAMService, IAMService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["IAMService:BaseUrl"]!);
            });
            // Add application services
            builder.Services.AddApplication(builder.Configuration);

            // Add infrastructure services
            builder.Services.AddInfrastructureServices(builder.Configuration);

            // Add shared services
            builder.Services.AddSharedServices(builder.Configuration);
            builder.Services.AddJWTAuthenticationScheme(builder.Configuration);
            // builder.Services.Configure<LockoutOptions>(builder.Configuration.GetSection("Lockout"));
            builder.Services.AddPrivilegePolicies();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Temporarily disable HTTPS redirection for gRPC testing
            // app.UseHttpsRedirection();

            app.UseSharePolicies();

            // JWT authentication disabled
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();


            // Map gRPC services
            app.MapGrpcService<Laboratory_Service.API.Services.PatientGrpcService>();

            app.Run();
        }
    }
}