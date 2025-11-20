using Microsoft.EntityFrameworkCore;
using Monitoring_Service.Application.HandleRawTestResult.Command;
using Monitoring_Service.Application.Interface;
using Monitoring_Service.Infastructure.Data;
using Monitoring_Service.Infastructure.RabbitMQ;
using Monitoring_Service.Infastructure.Repository;
using Monitoring_Service.Infrastructure.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel for Production - use PORT from environment variable (Render default)
if (builder.Environment.IsProduction())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        var port = Environment.GetEnvironmentVariable("PORT");
        var portNumber = !string.IsNullOrEmpty(port) ? int.Parse(port) : 8080;
        options.ListenAnyIP(portNumber);
    });
}

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHostedService<RabbitMQConsumer>();

builder.Services.AddScoped<IRawResultRepository, RawResultRepository>();
builder.Services.AddScoped<IRawResultRepository, RawResultRepository>();
builder.Services.AddScoped<ILaboratoryPublisher, LaboratoryPublisher>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(HandleRawTestResultCommand).Assembly));

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
    var schemaName = config["Database:Schema"] ?? "monitoring_service";
    
    // Set schema vào static property của AppDbContext
    Monitoring_Service.Infastructure.Data.AppDbContext.SchemaName = schemaName;

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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnet/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
