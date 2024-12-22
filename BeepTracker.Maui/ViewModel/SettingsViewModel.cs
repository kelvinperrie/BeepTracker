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

        private string _apiBasePath;
        private bool _attemptToSyncRecords;
        private string _birdListJson;

        public ObservableCollection<Bird> Birds { get; } = new();

        public SettingsViewModel(ISettingsService settingsService, ClientService clientService, RecordSyncService recordSyncService)
        {
            _settingsService = settingsService;
            _clientService = clientService;
            _recordSyncService = recordSyncService;

            _apiBasePath = _settingsService.ApiBasePath;
            _attemptToSyncRecords = _settingsService.AttemptToSyncRecords;
            _birdListJson = _settingsService.BirdListJson;

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

        [RelayCommand]
        public async Task SyncRecords()
        {
            var response = await _recordSyncService.UploadRecords();
            var message = "";
            var title = response.uploadFailureCount == 0 ? "Done" : "Error";
            if (response.totalRecordsAttempted == 0)
            {
                message = "No records are ready to be uploaded";
            }
            else
            {
                message = response.uploadFailureCount == 0 ? $"{response.totalRecordsAttempted} record(s) uploaded successfully." : $"{response.uploadFailureCount} out of {response.totalRecordsAttempted} records failed to upload! Check each record to see the error message.";
            }
            await Shell.Current.DisplayAlert("Done", message, "OK");
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

                var birdsJson = JsonSerializer.Serialize(Birds);
                BirdListJson = birdsJson;

                await Shell.Current.DisplayAlert("Done", $"Successfully updated the birds list - we received {Birds.Count} birds.", "OK");
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
