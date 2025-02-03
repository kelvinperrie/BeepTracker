using BeepTracker.Common.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using NLog;
using NLog.Web;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication;
using BeepTracker.Api.Security;
using BeepTracker.Api.Services;
using Asp.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using BeepTracker.Api.Swagger;

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
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(c => {
    c.OperationFilter<SwaggerDefaultValues>();
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                      new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "basic"
                            }
                        },
                        new string[] {}
                }
            });
});
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer();

builder.Services.AddDbContext<BeepTrackerDbContext>(x =>
{
    var connectionString = builder.Configuration.GetConnectionString("BeepTrackerConnection");
    //x.UseSqlServer(connectionString);
    x.UseNpgsql(connectionString);
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddTransient<UserService>();

builder.Logging
    .SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);


var app = builder.Build();

app.UseDeveloperExceptionPage();

//var logger = (ILogger<Program>?)app.Services.GetService(typeof(ILogger<Program>));
//logger.LogWarning("We're in the program");
//var connectionString = builder.Configuration.GetConnectionString("BeepTrackerConnection");
//logger.LogWarning("Connection string is " + connectionString);

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("../swagger/v1/swagger.json", "My API V1");
    // c.RoutePrefix = string.Empty;
});
//}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
