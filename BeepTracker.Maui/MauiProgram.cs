using BeepTracker.Maui.Services;
using Microsoft.Extensions.Logging;
using BeepTracker.ApiClient.IoC;
using BeepTracker.ApiClient;

namespace BeepTracker.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
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

            builder.Services.AddSingleton<BeepEntriesViewModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<ModelFactory>();
            builder.Services.AddSingleton<LocalPersistance>();

            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddTransient<BeepEntryDetailsViewModel>();
            builder.Services.AddTransient<StartPageViewModel>();
            builder.Services.AddTransient<DetailsPage>();
            builder.Services.AddTransient<StartPage>();
            builder.Services.AddTransient<InfoPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<ISettingsService, SettingsService>();


            // i detest this
            var app = builder.Build();
            var settings = (ISettingsService)app.Services.GetService(typeof(ISettingsService));
            var apiBasePath = settings.ApiBasePath;
            var clientService = (ClientService)app.Services.GetService(typeof(ClientService));
            // todo - if the user puts an invalid url into the text box then this this will break
            clientService.SetBaseAddress(apiBasePath);

            return app;
        }
    }
}
