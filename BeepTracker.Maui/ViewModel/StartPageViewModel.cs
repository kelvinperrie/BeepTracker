using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.Maui.ViewModel
{
    public partial class StartPageViewModel
    {
        public StartPageViewModel(ILogger<StartPageViewModel> logger)
        {
            logger.LogDebug("Start page DEBUG message");
            logger.LogWarning("Start page WARNING message");
            logger.LogInformation("Start page INFO message");
        }

        [RelayCommand]
        async Task GoToBeepRecordsPage()
        {
            await Shell.Current.GoToAsync("//"+nameof(MainPage), true, new Dictionary<string, object>{});
        }
    }


}
