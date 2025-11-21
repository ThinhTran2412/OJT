using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Simulator.API.Service;
using Simulator.Application.HostedService;
using Simulator.Application.Interface;
using Simulator.Application.SimulateRawData.Command;
using Simulator.Infastructure.Data;
using Simulator.Infastructure.RabbitMQ;
using Simulator.Infastructure.Repository;
using MediatR;
using System.Reflection;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel for Production - use PORT from environment variable (Render default)
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
            // Use Http1 instead of Http1AndHttp2 to avoid HTTPS configuration issues
            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
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

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Simulator Service API",
        Version = "v1",
        Description = "API for simulating laboratory test data"
    });
});

// Add gRPC services
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 6 * 1024 * 1024; // 6 MB
    options.MaxSendMessageSize = 6 * 1024 * 1024;    // 6 MB
});

// Add MediatR for CQRS pattern
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.RegisterServicesFromAssembly(typeof(SimulateRawDataCommandHandler).Assembly);
});

// Add gRPC Service
builder.Services.AddScoped<RawDataQueryGrpcService>();

// --- Configure RabbitMQ ---
builder.Services.Configure<Simulator.Infastructure.RabbitMQ.RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

// Factory Connection là Singleton
builder.Services.AddSingleton<IConnection>(sp =>
{
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<Simulator.Infastructure.RabbitMQ.RabbitMQSettings>>().Value;
    return Simulator.Infastructure.RabbitMQ.RabbitMQConfig.GetConnection(settings);
});

// Publisher là Singleton
builder.Services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

// --- Repository and DB Context ---
// Đăng ký Interface cho Repository
builder.Services.AddScoped<IRawTestResultRepository, RawTestResultRepository>();

// Đăng ký DbContext
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    // Ưu tiên lấy từ biến môi trường DATABASE_URL (Render, Railway, v.v.)
    var envConnection = Environment.GetEnvironmentVariable("DATABASE_URL");

    // Nếu không có thì lấy trong appsettings.json
    var connectionString = !string.IsNullOrEmpty(envConnection)
        ? ConvertPostgresUrlToConnectionString(envConnection)
        : config.GetConnectionString("DefaultConnection");

    // Lấy schema từ appsettings.json, mặc định là "simulator_service"
    var schema = config.GetValue<string>("Database:Schema") ?? "simulator_service";

    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", schema);
    });
});

// Add Hosted Service
builder.Services.AddHostedService<Simulator.Application.HostedService.RawDataSimulationService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middlewares
app.UseCors("AllowFrontend"); // Must be before UseAuthentication and UseAuthorization

// Map Controllers and gRPC Services
app.MapControllers();
app.MapGrpcService<RawDataQueryGrpcService>();

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
