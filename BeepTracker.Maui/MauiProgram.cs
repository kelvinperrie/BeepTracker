using BeepTracker.Maui.Services;
using Microsoft.Extensions.Logging;
using BeepTracker.ApiClient.IoC;
using BeepTracker.ApiClient;
using CommunityToolkit.Maui;
using MetroLog.MicrosoftExtensions;
using MetroLog.Operators;

namespace BeepTracker.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>().UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddClientService(x => x.BaseAddress = "http://10.0.2.2:5041");

            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
            builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
            builder.Services.AddSingleton<IMap>(Map.Default);

            builder.Services.AddSingleton<ModelFactory>();
            builder.Services.AddSingleton<LocalPersistance>();
            builder.Services.AddSingleton<RecordSyncService>(); 
            builder.Services.AddSingleton<ISettingsService, SettingsService>();

            builder.Services.AddSingleton<BeepEntriesViewModel>();
            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<SettingsPage>();

            builder.Services.AddTransient<BeepEntryDetailsViewModel>();
            builder.Services.AddTransient<DetailsPage>();
            
            builder.Services.AddSingleton<StartPageViewModel>();
            builder.Services.AddSingleton<StartPage>();
            
            builder.Services.AddSingleton<InfoPage>();

            builder.Services.AddAutoMapper(typeof(MauiProgram));

            builder.Logging
                .SetMinimumLevel(LogLevel.Debug)
                .AddTraceLogger(
                    options =>
                    {
                        options.MinLevel = LogLevel.Trace;
                        options.MaxLevel = LogLevel.Critical;
                    }) // Will write to the Debug Output
                .AddInMemoryLogger(         
                    options =>
                    {
                        options.MaxLines = 1024;
                        options.MinLevel = LogLevel.Trace;
                        options.MaxLevel = LogLevel.Critical;
                    }) // this is for displaying on the page
                .AddStreamingFileLogger(    
                    options =>
                    {
                        options.RetainDays = 5;
                        options.FolderPath = Path.Combine(
                            FileSystem.CacheDirectory,
                            "MetroLogs");
                    }); // this is for sharing the logs via the 'Share Logs' button

            builder.Services.AddSingleton(LogOperatorRetriever.Instance);

            var app = builder.Build();

            // i detest this
            var settings = (ISettingsService?)app.Services.GetService(typeof(ISettingsService));
            var apiBasePath = settings?.ApiBasePath;
            var clientService = (ClientService?)app.Services.GetService(typeof(ClientService));
            // todo - if the user puts an invalid url into the text box then this this will break
            clientService?.SetBaseAddress(apiBasePath??string.Empty);

            return app;
        }
    }
}
