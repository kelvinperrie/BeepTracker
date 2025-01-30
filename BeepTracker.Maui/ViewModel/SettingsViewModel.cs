using BeepTracker.ApiClient;
using BeepTracker.ApiClient.Models;
using BeepTracker.Maui.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using MetroLog.Maui;
using MetroLog;
using Microsoft.Extensions.Logging;
using MetroLog.Operators;
using static System.Net.Mime.MediaTypeNames;

namespace BeepTracker.Maui.ViewModel
{
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly ClientService _clientService;
        private readonly RecordSyncService _recordSyncService;
        private readonly IConnectivity _connectivity;
        private readonly ILogger<SettingsViewModel> _logger;
        private readonly LocalPersistance _localPersistance;

        private string _apiBasePath;
        private string _apiUsername;
        private string _apiPassword;
        private bool _attemptToSyncRecords;
        private string _birdListJson;

        public ObservableCollection<Bird> Birds { get; } = new();

        [ObservableProperty]
        public string? connectivityValue;

        public SettingsViewModel(ISettingsService settingsService, ClientService clientService, 
            RecordSyncService recordSyncService, IConnectivity connectivity, ILogger<SettingsViewModel> logger,
            LocalPersistance localPersistance)
        {
            _settingsService = settingsService;
            _clientService = clientService;
            _recordSyncService = recordSyncService;
            _connectivity = connectivity;
            _logger = logger;
            _localPersistance = localPersistance;

            _apiBasePath = _settingsService.ApiBasePath;
            _apiUsername = _settingsService.ApiUsername;
            _apiPassword = _settingsService.ApiPassword;
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
        public string ApiUsername
        {
            get => _apiUsername;
            set
            {
                _apiUsername = value;
                _settingsService.ApiUsername = _apiUsername;
            }
        }
        public string ApiPassword
        {
            get => _apiPassword;
            set
            {
                _apiPassword = value;
                _settingsService.ApiPassword = _apiPassword;
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
            // todo maybe make this fancier at some point
            ConnectivityValue = currentNetworkAccess.ToString();
        }

        [RelayCommand]
        public async Task OpenLogsPage()
        {
            var logController = new LogController();

            // will show the MetroLogPage by default
            logController.GoToLogsPageCommand.Execute(null);
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
                _logger.LogInformation("About to attempt to sync records");
                IsBusy = true;

                _clientService.SetUsernameAndPassword(ApiUsername, ApiPassword);
                _logger.LogInformation("Calling record sync service");
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
                _logger.LogError(ex, "Error in sync records");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                _logger.LogInformation("Record sync completed");
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
                _logger.LogInformation("About to attempt to get birds");
                IsBusy = true;

                _clientService.SetUsernameAndPassword(ApiUsername, ApiPassword);
                _logger.LogInformation("Calling getbirds via API service");
                var birds = await _clientService.GetBirds();

                if (Birds.Any())
                {
                    _logger.LogInformation($"Retrieved {Birds.Count()} birds from API");
                    Birds.Clear();
                }

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
                _logger.LogError(ex, "Error while getting birds");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task ViewBirds()
        {

            var birdNames = String.Join(", ", _settingsService.BirdListFromDatabase.Select(b => b.Name));
            var title = $"{_settingsService.BirdListFromDatabase.Count} bird(s) found";
            await Shell.Current.DisplayAlert(title, $"Birds are: {birdNames}\nJson is: {BirdListJson}", "OK");
        }

        [RelayCommand]
        public async Task<string> GenerateCompressedBeepRecordFile()
        {

            var filePath = await _localPersistance.CreateCompressedBeepRecordsFile();
            return filePath;

            //var compressedRecords = await _localPersistance.GetCompressedBeepRecords();

            //var fileName = $"beeprecords.zip";
            //var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            //MemoryStream? compressedLogs = (MemoryStream)compressedRecords; // await _logController.GetCompressedLogs();

            //if (compressedLogs == null)
            //{
            //    return;
            //}

            //await using (var fileStream = new FileStream(filePath, FileMode.Create))
            //{
            //    await compressedLogs.CopyToAsync(fileStream);
            //}

            //await Share.Default.RequestAsync(new ShareFileRequest
            //{
            //    Title = "Sharing compressed beeprecords",
            //    File = new ShareFile(filePath)
            //});

            //await Share.Default.RequestAsync(new ShareTextRequest
            //{
            //    Text = "Hello! This is the text being shared",
            //    Title = "This is the title"
            //});


            //var compressedLogs = await logCompressor.GetCompressedLogs();

            //return new[]
            //{
            //    ErrorAttachmentLog.AttachmentWithBinary(
            //        compressedLogs.ToArray(),
            //        "logs.zip",
            //        "application/x-zip-compressed"),
            //};
        }

    }
}
