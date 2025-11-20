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

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel for Production - use PORT from environment variable (Render default)
if (builder.Environment.IsProduction())
{
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

builder.Services.AddScoped<RawDataQueryGrpcService>();
builder.Services.AddScoped<IRabbitMQPublisher,RabbitMQPublisher>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SimulateRawDataCommandHandler>());

// --- 3. Repository and DB Context ---
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
    var schemaName = config["Database:Schema"] ?? "simulator_service";
    
    // Set schema vào static property của AppDbContext
    Simulator.Infastructure.Data.AppDbContext.SchemaName = schemaName;

    options.UseNpgsql(connectionString);
});

static string ConvertPostgresUrlToConnectionString(string url)
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

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 6 * 1024 * 1024; 
    options.MaxSendMessageSize = 6 * 1024 * 1024;    
});
// --- 4. Configure RabbitMQ ---
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

// Factory Connection là Singleton
builder.Services.AddSingleton<IConnection>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
    return RabbitMQConfig.GetConnection(settings);
});
// Publisher là Singleton
builder.Services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

// Configure Kestrel for Development (Production already configured above)
if (!builder.Environment.IsProduction())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        // Setup a HTTP/2 endpoint without TLS.
        options.ListenLocalhost(7003, o => o.Protocols =
             Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
    });
}
// --- 5. Add BackgroundService ---
builder.Services.AddHostedService<RawDataSimulationService>();
// --- 6. Add Controllers & Swagger  ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

 app.UseSwagger();
 app.UseSwaggerUI();


app.UseRouting();
app.UseAuthorization(); 

// --- 7. Map Endpoints ---
app.MapGrpcService<RawDataQueryGrpcService>();
app.MapControllers();

app.Run();