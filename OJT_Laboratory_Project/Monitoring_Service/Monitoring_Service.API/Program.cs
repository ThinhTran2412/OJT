using Microsoft.EntityFrameworkCore;
using Monitoring_Service.Application.HandleRawTestResult.Command;
using Monitoring_Service.Application.Interface;
using Monitoring_Service.Infastructure.Data;
using Monitoring_Service.Infastructure.RabbitMQ;
using Monitoring_Service.Infastructure.Repository;
using Monitoring_Service.Infrastructure.RabbitMQ;
using MediatR;
using System.Reflection;

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

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Monitoring Service API",
        Version = "v1",
        Description = "API for monitoring laboratory test results"
    });
});

// Add MediatR for CQRS pattern
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.RegisterServicesFromAssembly(typeof(HandleRawTestResultCommand).Assembly);
});

// Add Hosted Services
builder.Services.AddHostedService<RabbitMQConsumer>();

// Add Repositories
builder.Services.AddScoped<IRawResultRepository, RawResultRepository>();
builder.Services.AddScoped<ILaboratoryPublisher, LaboratoryPublisher>();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    // Ưu tiên lấy từ biến môi trường DATABASE_URL (Render, Railway, v.v.)
    var envConnection = Environment.GetEnvironmentVariable("DATABASE_URL");

    // Nếu không có thì lấy trong appsettings.json
    var connectionString = !string.IsNullOrEmpty(envConnection)
        ? ConvertPostgresUrlToConnectionString(envConnection)
        : config.GetConnectionString("DefaultConnection");

    // Lấy schema từ appsettings.json, mặc định là "monitoring_service"
    var schema = config.GetValue<string>("Database:Schema") ?? "monitoring_service";

    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", schema);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middlewares
app.UseCors("AllowFrontend"); // Must be before UseAuthentication and UseAuthorization

// Map Controllers
app.MapControllers();

app.Run();

// Helper function to convert PostgreSQL URL to connection string
static string ConvertPostgresUrlToConnectionString(string postgresUrl)
{
    if (string.IsNullOrWhiteSpace(postgresUrl))
        return string.Empty;

    var uri = new Uri(postgresUrl);
    var userInfo = uri.UserInfo.Split(':');

    var connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={Uri.UnescapeDataString(userInfo[1])}";

    // Add SSL mode for external connections
    if (uri.Host.Contains(".render.com") || uri.Host.Contains(".railway.app"))
    {
        connectionString += ";SSL Mode=Require;Trust Server Certificate=true";
    }

    return connectionString;
}
