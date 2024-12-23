//using BeepTracper.Maui.Services;
using BeepTracker.Maui.Services;
using MetroLog;
using Microsoft.Extensions.Logging;
using Newtonsoft;
using Newtonsoft.Json;

namespace BeepTracker.Maui.ViewModel;

public partial class BeepEntriesViewModel : BaseViewModel
{

    public ObservableCollection<BeepRecord> BeepRecords { get; } = new();

    private readonly IConnectivity _connectivity;
    private readonly IGeolocation _geolocation;
    private readonly ModelFactory _modelFactory;
    private readonly LocalPersistance _localPersistance;
    private readonly ILogger<BeepEntriesViewModel> _logger;

    [ObservableProperty]
    bool isRefreshing;

    public BeepEntriesViewModel(IConnectivity connectivity, IGeolocation geolocation, 
        ModelFactory modelFactory, LocalPersistance localPersistance,
        ILogger<BeepEntriesViewModel> logger)
    {
        _connectivity = connectivity;
        _geolocation = geolocation;
        _modelFactory = modelFactory;
        _localPersistance = localPersistance;
        _logger = logger;
    }


    [RelayCommand]
    async Task CreateNewBeepRecord()
    {
        await Shell.Current.GoToAsync(nameof(DetailsPage), true, new Dictionary<string, object>
            {
                {"BeepRecordPassed", _modelFactory.CreateBeepRecord() }
            });
    }

    [RelayCommand]
    public async Task GetBeepRecordsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            IsRefreshing = true;

            var records = _localPersistance.GetBeepRecords();

            if(BeepRecords.Any())
                BeepRecords.Clear();

            foreach(var record in records)
            {
                BeepRecords.Add(record);
            }
            _logger.LogInformation($"Found {records.Count} beep records to display");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting list of beep records");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    async Task GoToBeepDetails(BeepRecord beepRecord)
    {
        if (beepRecord == null)
            return;

        await Shell.Current.GoToAsync(nameof(DetailsPage), true, new Dictionary<string, object>
        {
            {"BeepRecordPassed", beepRecord }
        });
    }

}
