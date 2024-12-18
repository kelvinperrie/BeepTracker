using BeepTracker.ApiClient;
using BeepTracker.ApiClient.Models;
using BeepTracker.Maui.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.Maui.ViewModel
{
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private string _apiBasePath;
        private readonly ClientService _clientService;

        public ObservableCollection<Bird> Birds { get; } = new();

        public SettingsViewModel(ISettingsService settingsService, ClientService clientService)
        {
            _settingsService = settingsService;
            _clientService = clientService;

            _apiBasePath = _settingsService.ApiBasePath;
        }

        public string ApiBasePath
        {
            get => _apiBasePath;
            set
            {
                //SetProperty(ref _apiBasePath, value);
                _apiBasePath = value;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    _settingsService.ApiBasePath = _apiBasePath;
                }
            }
        }

        [RelayCommand]
        public async Task GetBirds()
        {

            if (IsBusy) return;

            try
            {
                IsBusy = true;


                var birds = await _clientService.GetBirds();

                if (Birds.Any())
                    Birds.Clear();

                foreach (var record in birds)
                {
                    Birds.Add(record);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to!!!!!!!!!: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }


        }

    }
}
