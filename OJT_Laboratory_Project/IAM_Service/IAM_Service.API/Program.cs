using IAM_Service.Application;
using IAM_Service.Infrastructure;
using SharedLibrary.DependencyInjection;
using IAM_Service.Application.Common.Security;
using IAM_Service.API.Services;
using MyCompany.Authorization.Setup;


namespace IAM_Service.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Kestrel for Production - use PORT from environment variable (Render default)
            // Disable HTTPS in production - Render handles HTTPS at the load balancer level
            if (builder.Environment.IsProduction())
            {
                builder.WebHost.ConfigureKestrel(options =>
                {
                    // Configure only HTTP endpoint using PORT from environment variable
                    // This overrides any HTTPS configuration from appsettings.json
                    var port = Environment.GetEnvironmentVariable("PORT");
                    var portNumber = !string.IsNullOrEmpty(port) ? int.Parse(port) : 8080;
                    options.ListenAnyIP(portNumber, listenOptions =>
                    {
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

            // Add controllers and Swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "IAM Service API",
                    Version = "v1",
                    Description = "API for Identity and Access Management"
                });

                // Include all XML comments if available
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            // Add gRPC (with detailed errors and message size limits)
            builder.Services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaxReceiveMessageSize = 6 * 1024 * 1024; // 6 MB
                options.MaxSendMessageSize = 6 * 1024 * 1024;    // 6 MB
            });

            // Register internal gRPC services
            builder.Services.AddScoped<UserGrpcService>();
            //builder.Services.AddSingleton<PatientGrpcClientService>();

            // Register application and infrastructure layers
            builder.Services.AddApplication();
            builder.Services.AddAutoMapper(typeof(IAM_Service.Application.Common.Mappings.AutoMapperProfile));

            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddSharedServices(builder.Configuration);
            builder.Services.AddPrivilegePolicies();

            builder.Services.Configure<LockoutOptions>(builder.Configuration.GetSection("Lockout"));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Middlewares
            app.UseSharePolicies();
            app.UseAuthentication();
            app.UseAuthorization();

            // Map Controllers and gRPC Services
            app.MapControllers();
            app.MapGrpcService<UserGrpcService>();

            app.Run();
        }
    }
}
