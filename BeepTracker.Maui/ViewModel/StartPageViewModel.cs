using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.Maui.ViewModel
{
    public partial class StartPageViewModel
    {
        public StartPageViewModel()
        {
        }

        [RelayCommand]
        async Task GoToBeepRecordsPage()
        {
            await Shell.Current.GoToAsync(nameof(MainPage), true, new Dictionary<string, object>{});
        }
    }


}
