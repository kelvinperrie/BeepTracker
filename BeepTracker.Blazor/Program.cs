using AutoMapper;
using BeepTracker.Blazor.Business;
using BeepTracker.Blazor.Components;
using BeepTracker.Common.Models;
using BeepTracker.Common.Dtos;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using BeepTracker.Common.Services;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddDbContext<BeepTrackerDbContext>(x =>
{
    var connectionString = builder.Configuration.GetConnectionString("BeepTrackerConnection");
    //x.UseSqlServer(connectionString);
    x.UseNpgsql(connectionString);
});

builder.Services.AddTransient<IOrganisationService, OrganisationService>();
builder.Services.AddTransient<IBirdService, BirdService>();
builder.Services.AddTransient<IBeepRecordService, BeepRecordService>();
builder.Services.AddTransient<ModelFactory>();

builder.Services.AddAutoMapper(typeof(Program));

//var configuration = new MapperConfiguration(cfg =>
//{
//    cfg.CreateMap<Bird, BirdDto>().ReverseMap();
//    //cfg.CreateMap<Bar, BarDto>();
//});
//var mapper = configuration.CreateMapper();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
