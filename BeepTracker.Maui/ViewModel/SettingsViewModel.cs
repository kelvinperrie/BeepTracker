using BeepTracker.ApiClient;
using BeepTracker.ApiClient.Models;
using BeepTracker.Maui.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace BeepTracker.Maui.ViewModel
{
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly ClientService _clientService;
        private readonly RecordSyncService _recordSyncService;
        private readonly IConnectivity _connectivity;

        private string _apiBasePath;
        private bool _attemptToSyncRecords;
        private string _birdListJson;

        public ObservableCollection<Bird> Birds { get; } = new();

        [ObservableProperty]
        public string? connectivityValue;

        public SettingsViewModel(ISettingsService settingsService, ClientService clientService, 
            RecordSyncService recordSyncService, IConnectivity connectivity)
        {
            _settingsService = settingsService;
            _clientService = clientService;
            _recordSyncService = recordSyncService;
            _connectivity = connectivity;

            _apiBasePath = _settingsService.ApiBasePath;
            _attemptToSyncRecords = _settingsService.AttemptToSyncRecords;
            _birdListJson = _settingsService.BirdListJson;

            _connectivity.ConnectivityChanged += OnConnectivityChanged;
            UpdateConnectivityStatus(_connectivity.NetworkAccess);
        }


        public string ApiBasePath
        {
            get => _apiBasePath;
            set
            {
                _apiBasePath = value;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    _settingsService.ApiBasePath = _apiBasePath;
                }
            }
        }
        public bool AttemptToSyncRecords
        {
            get => _attemptToSyncRecords;
            set
            {
                _attemptToSyncRecords = value;
                _settingsService.AttemptToSyncRecords = _attemptToSyncRecords;
            }
        }

        public string BirdListJson
        {
            get => _birdListJson;
            set
            {
                _birdListJson = value;
                _settingsService.BirdListJson = _birdListJson;
            }
        }

        private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            UpdateConnectivityStatus(e.NetworkAccess);
        }

        private void UpdateConnectivityStatus(NetworkAccess currentNetworkAccess) 
        {
            // maybe make this fancier at some point
            ConnectivityValue = currentNetworkAccess.ToString();
        }


        [RelayCommand]
        public async Task SyncRecords()
        {
            if (IsBusy)
            {
                await Shell.Current.DisplayAlert("Warning", "The page is already processing a request.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var response = await _recordSyncService.UploadRecords();
                var message = "";
                var title = response.uploadFailureCount == 0 ? "Done" : "Error";
                if (response.totalRecordsAttempted == 0)
                {
                    message = "There are no records in a state (created or updated) that need to be uploaded.";
                }
                else
                {
                    message = response.uploadFailureCount == 0 ? $"{response.totalRecordsAttempted} record(s) uploaded successfully." : $"{response.uploadFailureCount} out of {response.totalRecordsAttempted} records failed to upload! Check each record to see the error message.";
                }
                await Shell.Current.DisplayAlert("Done", message, "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task GetBirds()
        {

            if (IsBusy)
            {
                await Shell.Current.DisplayAlert("Warning", "The page is already processing a request.", "OK");
                return;
            }

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

                var birdsJson = JsonSerializer.Serialize(Birds);
                BirdListJson = birdsJson;

                await Shell.Current.DisplayAlert("Done", $"Successfully updated the birds list - we received {Birds.Count} birds.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }


        }

    }
}
