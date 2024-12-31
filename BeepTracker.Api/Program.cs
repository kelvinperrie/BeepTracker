using BeepTracker.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using NLog;
using NLog.Web;
using Microsoft.ApplicationInsights.Extensibility;

#if DEBUG
TelemetryConfiguration.Active.DisableTelemetry = true;
#endif

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// Add OpenTelemetry and configure it to use Azure Monitor.

#if RELEASE
builder.Services.AddOpenTelemetry().UseAzureMonitor();
#endif

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BeepTrackerDbContext>(x =>
{
    var connectionString = builder.Configuration.GetConnectionString("BeepTrackerConnection");
    //x.UseSqlServer(connectionString);
    x.UseNpgsql(connectionString);
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Logging
    .SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);

var app = builder.Build();

app.Logger.LogInformation("HELLLLLLLLO");
//var logger = (ILogger<Program>?)app.Services.GetService(typeof(ILogger<Program>));
//logger.LogWarning("We're in the program");
//var connectionString = builder.Configuration.GetConnectionString("BeepTrackerConnection");
//logger.LogWarning("Connection string is " + connectionString);

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
