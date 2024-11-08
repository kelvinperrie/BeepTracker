//using BeepTracper.Maui.Services;
using Newtonsoft;
using Newtonsoft.Json;

namespace BeepTracker.Maui.ViewModel;

public partial class BeepEntriesViewModel : BaseViewModel
{
    //public ObservableCollection<Monkey> Monkeys { get; } = new();

    public ObservableCollection<BeepRecord> BeepRecords { get; } = new();

    //MonkeyService monkeyService;
    IConnectivity connectivity;
    IGeolocation geolocation;

    public BeepEntriesViewModel(IConnectivity connectivity, IGeolocation geolocation)
    {
        Title = "Monkey Finder";
        //this.monkeyService = monkeyService;
        this.connectivity = connectivity;
        this.geolocation = geolocation;

        var basePath = FileSystem.Current.AppDataDirectory;
        var path = Path.Combine(basePath, "beeprecords");

        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        foreach(var filepath in Directory.EnumerateFiles(path).OrderDescending())
        {
            var contents = File.ReadAllText(filepath);
            BeepRecord beepRecord = JsonConvert.DeserializeObject<BeepRecord>(contents);
            beepRecord.Filename = Path.GetFileName(filepath);
            BeepRecords.Add(beepRecord);
        }


    }

    [ObservableProperty]
    bool isRefreshing;

    [RelayCommand]
    async Task CreateNewBeepRecord()
    {
        await Shell.Current.GoToAsync(nameof(DetailsPage), true, new Dictionary<string, object>
        {
            {"BeepRecord", new BeepRecord() }
        });
    }

    [RelayCommand]
    async Task GetMonkeysAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("No connectivity!",
                    $"Please check internet and try again.", "OK");
                return;
            }

            IsBusy = true;
            //var monkeys = await monkeyService.GetMonkeys();

            //if(Monkeys.Count != 0)
            //    Monkeys.Clear();
                
            //foreach(var monkey in monkeys)
            //    Monkeys.Add(monkey);

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get monkeys: {ex.Message}");
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
            {"BeepRecord", beepRecord }
        });
    }

    //[RelayCommand]
    //async Task GoToDetails(Monkey monkey)
    //{
    //    if (monkey == null)
    //    return;

    //    await Shell.Current.GoToAsync(nameof(DetailsPage), true, new Dictionary<string, object>
    //    {
    //        {"Monkey", monkey }
    //    });
    //}

    //[RelayCommand]
    //async Task GetClosestMonkey()
    //{
    //    if (IsBusy || Monkeys.Count == 0)
    //        return;

    //    try
    //    {
    //        // Get cached location, else get real location.
    //        var location = await geolocation.GetLastKnownLocationAsync();
    //        if (location == null)
    //        {
    //            location = await geolocation.GetLocationAsync(new GeolocationRequest
    //            {
    //                DesiredAccuracy = GeolocationAccuracy.Medium,
    //                Timeout = TimeSpan.FromSeconds(30)
    //            });
    //        }

    //        // Find closest monkey to us
    //        var first = Monkeys.OrderBy(m => location.CalculateDistance(
    //            new Location(m.Latitude, m.Longitude), DistanceUnits.Miles))
    //            .FirstOrDefault();

    //        await Shell.Current.DisplayAlert("", first.Name + " " +
    //            first.Location, "OK");

    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($"Unable to query location: {ex.Message}");
    //        await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
    //    }
    //}
}
