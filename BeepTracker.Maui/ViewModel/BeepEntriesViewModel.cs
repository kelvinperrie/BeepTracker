//using BeepTracper.Maui.Services;
using BeepTracker.Maui.Services;
using Newtonsoft;
using Newtonsoft.Json;

namespace BeepTracker.Maui.ViewModel;

public partial class BeepEntriesViewModel : BaseViewModel
{

    public ObservableCollection<BeepRecord> BeepRecords { get; } = new();

    IConnectivity connectivity;
    IGeolocation geolocation;
    ModelFactory modelFactory;
    LocalPersistance localPersistance;

    [ObservableProperty]
    bool isRefreshing;

    public BeepEntriesViewModel(IConnectivity connectivity, IGeolocation geolocation, 
        ModelFactory modelFactory, LocalPersistance localPersistance)
    {
        Title = "asdfasdfasdf";
        this.connectivity = connectivity;
        this.geolocation = geolocation;
        this.modelFactory = modelFactory;
        this.localPersistance = localPersistance;

        //Shell.Current.DisplayAlert("Error!", "test", "OK");

        GetBeepRecordsAsync(); //.Wait();
    }

    [RelayCommand]
    async Task CreateNewBeepRecord()
    {
        await Shell.Current.GoToAsync(nameof(DetailsPage), true, new Dictionary<string, object>
            {
                {"BeepRecordPassed", modelFactory.CreateBeepRecord() }
            });
    }

    [RelayCommand]
    public async Task GetBeepRecordsAsync()
    {
        if (IsBusy) return;

        try
        {
            //if (connectivity.NetworkAccess != NetworkAccess.Internet)
            //{
            //    await Shell.Current.DisplayAlert("No connectivity!",
            //        $"Please check internet and try again.", "OK");
            //    return;
            //}

            IsBusy = true;

            var records = localPersistance.GetBeepRecords();

            if(BeepRecords.Any())
                BeepRecords.Clear();

            foreach(var record in records)
            {
                BeepRecords.Add(record);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get beep records: {ex.Message}");
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
