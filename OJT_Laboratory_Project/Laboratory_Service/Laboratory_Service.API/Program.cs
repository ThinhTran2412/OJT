using Laboratory_Service.Application;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.Services;
using Laboratory_Service.Infrastructure;
using Laboratory_Service.Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MyCompany.Authorization.Setup;
using SharedLibrary.DependencyInjection;
using System.Linq;

namespace Laboratory_Service.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create a builder for the web application.
            var builder = WebApplication.CreateBuilder(args);

            // Configure Kestrel for Production FIRST - before any other services
            // Disable HTTPS in production - Render handles HTTPS at the load balancer level
            if (builder.Environment.IsProduction())
            {
                // Remove HTTPS endpoint completely from configuration before Kestrel loads it
                // This prevents HTTPS endpoint from appsettings.json from being loaded
                var httpsSection = builder.Configuration.GetSection("Kestrel:Endpoints:Https");
                if (httpsSection.Exists())
                {
                    // Remove all child keys from HTTPS section to completely remove it
                    foreach (var child in httpsSection.GetChildren().ToList())
                    {
                        builder.Configuration[$"Kestrel:Endpoints:Https:{child.Key}"] = null;
                    }
                    // Also remove the section itself if possible
                    builder.Configuration["Kestrel:Endpoints:Https"] = null;
                }
                
                // Use UseKestrel instead of ConfigureKestrel to completely replace configuration
                builder.WebHost.UseKestrel(options =>
                {
                    // Clear all existing endpoints and configure only HTTP endpoint
                    // This completely overrides any HTTPS configuration from appsettings.json
                    var port = Environment.GetEnvironmentVariable("PORT");
                    var portNumber = !string.IsNullOrEmpty(port) ? int.Parse(port) : 8080;
                    options.ListenAnyIP(portNumber, listenOptions =>
                    {
                        // Use Http1AndHttp2 to support gRPC over HTTP/2
                        // This allows gRPC clients to connect via HTTP public URL with HTTP/2 unencrypted support
                        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
                    });
                });
                
                // Disable HTTPS redirection in production
                builder.Services.Configure<Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionOptions>(options =>
                {
                    options.RedirectStatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = null; // Disable HTTPS redirection
                });
            }

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

            // Configure CORS
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? new[] { "https://front-end-fnfs.onrender.com", "http://localhost:5173", "http://localhost:3000" };

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // Add gRPC services
            builder.Services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaxReceiveMessageSize = 6 * 1024 * 1024; // 6 MB
                options.MaxSendMessageSize = 6 * 1024 * 1024;    // 6 MB
            });

            builder.Services.AddSingleton<ITokenService, IAMTokenService>();

            builder.Services.AddHttpClient<IIAMService, IAMService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["IAMService:BaseUrl"]!);
                client.Timeout = TimeSpan.FromSeconds(30); // 30 second timeout for IAM Service calls
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

            // CORS middleware - MUST be first, before any other middleware
            // This ensures CORS headers are added to all responses including preflight OPTIONS requests
            app.UseCors("AllowFrontend");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Exception handler for production - must preserve CORS headers in error responses
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;
                        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                        
                        // Log the exception with full details
                        if (exception != null)
                        {
                            logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                        }
                        else
                        {
                            logger.LogError("Unknown error occurred");
                        }
                        
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        
                        // In production, return generic message but log details
                        // CORS headers are already added by UseCors middleware above
                        var response = new { message = "An error occurred while processing your request." };
                        await context.Response.WriteAsJsonAsync(response);
                    });
                });
            }

            app.UseSharePolicies();
            app.UseAuthentication();
            app.UseAuthorization();

            // Map Controllers and gRPC Services
            app.MapControllers();
            app.MapGrpcService<Laboratory_Service.API.Services.PatientGrpcService>();

            app.Run();
        }
    }
}