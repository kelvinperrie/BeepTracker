using BeepTracker.Maui.Services;
using Microsoft.Extensions.Logging;

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

            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
            builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
            builder.Services.AddSingleton<IMap>(Map.Default);

            builder.Services.AddSingleton<BeepEntriesViewModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<ModelFactory>();
            builder.Services.AddSingleton<LocalPersistance>();

            builder.Services.AddTransient<BeepEntryDetailsViewModel>();
            builder.Services.AddTransient<StartPageViewModel>();
            builder.Services.AddTransient<DetailsPage>();
            builder.Services.AddTransient<StartPage>();
            builder.Services.AddTransient<InfoPage>();

            return builder.Build();
        }
    }
}
