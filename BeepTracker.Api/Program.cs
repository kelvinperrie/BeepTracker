using BeepTracker.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Azure.Monitor.OpenTelemetry.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

// Add OpenTelemetry and configure it to use Azure Monitor.
builder.Services.AddOpenTelemetry().UseAzureMonitor();

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
    .SetMinimumLevel(LogLevel.Debug);

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
