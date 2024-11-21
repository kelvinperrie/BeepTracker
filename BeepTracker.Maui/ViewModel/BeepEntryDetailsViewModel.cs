using BeepTracker.Maui.Services;
using Newtonsoft.Json;
using System.Windows.Input;

namespace BeepTracker.Maui.ViewModel;

[QueryProperty(nameof(BeepRecord), "BeepRecordPassed")]
public partial class BeepEntryDetailsViewModel : BaseViewModel, INotifyPropertyChanged
{
    //public ObservableCollection<int> Beeps { get; } = new();
    //public RelayCommand Add1Command { get; private set; }
    //public RelayCommand Subtract1Command { get; private set; }
    public RelayCommand EnterNumberCommand { get; private set; }
    public ICommand BeatsPerMinuteClickedCommand { get; private set; }

    public ICommand ClearCommand { private set; get; }
    public ICommand BackspaceCommand { private set; get; }
    public ICommand DigitCommand { private set; get; }


    LocalPersistance localPersistance;

    public event PropertyChangedEventHandler PropertyChanged;


    public int SelectedBeepEntryIndex;
    public bool UserHasEnteredDigitIntoSelectedBeepEntry = false;

    IMap map;

    private CancellationTokenSource _cancelTokenSource; // cancellation tocken for the request to the device to look up the location
    public bool isCheckingLocation;     // tracks whether we are currently looking up the location

    public bool IsCheckingLocation
    {
        get
        {
            return isCheckingLocation;
        }
        set
        {
            if (isCheckingLocation != value)
            {
                isCheckingLocation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsCheckingLocation"));
            }
        }
    }

    BeepRecord beepRecord;
    public BeepRecord BeepRecord
    {
        get
        {
            return beepRecord;
        }
        set
        {
            if (beepRecord != value)
            {
                beepRecord = value;
                // mark the first beep entry as being the selected one
                beepRecord.BeepEntries.ForEach(be => be.Selected = false);  // this is needed for when a records is updated and then reopened from list page
                beepRecord.BeepEntries[0].Selected = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BeepRecord"));
            }
        }
    }

    public BeepEntryDetailsViewModel(IMap map, LocalPersistance localPersistance)
    {
        this.map = map;
        this.localPersistance = localPersistance;

        SelectedBeepEntryIndex = 0;

        //Add1Command = new RelayCommand(() => 
        //        BeepRecord.BeepEntries[SelectedBeepEntryIndex].Value++
        //    );
        //Subtract1Command = new RelayCommand(() => BeepRecord.BeepEntries[SelectedBeepEntryIndex].Value--);


        BeatsPerMinuteClickedCommand = new Command<string>(
            execute: (string arg) =>
            {
                int converted = int.Parse(arg);
                if (BeepRecord.BeatsPerMinute == converted)
                {
                    BeepRecord.BeatsPerMinute = null;
                }
                else
                {
                    BeepRecord.BeatsPerMinute = converted;
                }
            },
            canExecute: (string arg) => { return true; });

        ClearCommand = new Command(
            execute: () =>
            {
                BeepRecord.BeepEntries[SelectedBeepEntryIndex].Value = null;
            });
        DigitCommand = new Command<string>(
            execute: (string arg) =>
            {
                // if the user has just gone into this beep entry and hit a number key, then we want to overwrite the
                // contents of the beep entry
                // if they have hit multiple number keys then we want to add them to the beep entry
                if (UserHasEnteredDigitIntoSelectedBeepEntry)
                {
                    arg = BeepRecord.BeepEntries[SelectedBeepEntryIndex].Value.ToString() + arg;

                }
                int converted = int.Parse(arg);
                BeepRecord.BeepEntries[SelectedBeepEntryIndex].Value = converted;
                UserHasEnteredDigitIntoSelectedBeepEntry = true;
                HandleBeepEntryChange();
            },
            canExecute: (string arg) =>
            {
                return true; // !(arg == "." && Entry.Contains("."));
            });
    }

    public void HandleBeepEntryChange()
    {
        // if the first beep entry is changed then update the recorded time
        if(SelectedBeepEntryIndex == 0)
        {
            BeepRecord.RecordedDate = DateTime.Now.Date;
            BeepRecord.RecordedTime = DateTime.Now.TimeOfDay;
        }
    }


    [RelayCommand]
    public void Add1ToCurrentBeepEntry()
    {
        if(BeepRecord.BeepEntries[SelectedBeepEntryIndex].Value == null)
        {
            BeepRecord.BeepEntries[SelectedBeepEntryIndex].Value = 1;
        } else {
            BeepRecord.BeepEntries[SelectedBeepEntryIndex].Value++;
        }
        HandleBeepEntryChange();
    }

    [RelayCommand]
    public void Subtract1FromCurrentBeepEntry()
    {
        BeepRecord.BeepEntries[SelectedBeepEntryIndex].Value--;
        HandleBeepEntryChange();
    }

    [RelayCommand]
    public void SaveBeepRecord()
    {
        localPersistance.SaveBeepRecord(BeepRecord);
        Shell.Current.DisplayAlert("Save completed", "The beep record has been saved.", "OK");
    }

    [RelayCommand]
    public async Task GoToBeepEntriesPage()
    {

        // check to see if data has changed

        if(BeepRecord.Filename == null)
        {
            // this has never been saved as a file before
            bool answer = await Shell.Current.DisplayAlert("Unsaved data?", "Are you sure you want to return to the list page without saving your data?", "Yes", "No");

            if (answer)
            {
                // do navigation without saving data
                await Shell.Current.GoToAsync("MainPage", true);
                return;
            } else
            {
                return;
            }
        }


        var beepRecordFromFile = localPersistance.GetBeepRecordByFilename(BeepRecord.Filename);

        if (!BeepRecord.ContentEquals(beepRecordFromFile))
        {
            // there have been changes to the data
            bool answer = await Shell.Current.DisplayAlert("Unsaved data?", "Are you sure you want to return to the list page without saving your data?", "Yes", "No");

            if (answer)
            {
                // do navigation without saving data
                // overwrite the beep record with the one we just got from the file
                BeepRecord = beepRecordFromFile;
                // I don't know why this doesn't work ...
                // when you return to the list it keeps the changed version of the record
                // for now tell the user to refresh there list, should figure this out one day
                await Shell.Current.DisplayAlert("Refresh needed", "When you return to the beep record list make sure you refresh it to reload records from file.", "OK");
                await Shell.Current.GoToAsync("MainPage", true);
            } else
            {
                // save the data? or just let them push the save button???
            }
        } else
        {
            // no changes
            await Shell.Current.GoToAsync("MainPage", true);
        }
    }

    [RelayCommand]
    public void NextBeepEntry()
    {
        if (SelectedBeepEntryIndex == beepRecord.BeepEntries.Count - 1) return;
        beepRecord.BeepEntries[SelectedBeepEntryIndex].Selected = false;
        SelectedBeepEntryIndex++;
        beepRecord.BeepEntries[SelectedBeepEntryIndex].Selected = true;
        UserHasEnteredDigitIntoSelectedBeepEntry = false;
    }
    [RelayCommand]
    public void PreviousBeepEntry()
    {
        if (SelectedBeepEntryIndex == 0) return;
        beepRecord.BeepEntries[SelectedBeepEntryIndex].Selected = false;
        SelectedBeepEntryIndex--;
        beepRecord.BeepEntries[SelectedBeepEntryIndex].Selected = true;
        UserHasEnteredDigitIntoSelectedBeepEntry = false;
    }

    [RelayCommand]
    public void UpdateLatLong()
    {
        // we call this async and don't wait for a response so that we don't lock up the UI
        GetCurrentLocation();
    }


    public async Task GetCurrentLocation()
    {
        try
        {
            if (IsCheckingLocation)
            {
                await Shell.Current.DisplayAlert("Already looking!", "We've already requested the location from the device and are waiting for an answer.", "OK");
            }

            IsCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if (location != null)
            {
                Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                BeepRecord.Latitude = location.Latitude.ToString();
                BeepRecord.Longitude = location.Longitude.ToString();

            }
        }
        // Catch one of the following exceptions:
        //   FeatureNotSupportedException
        //   FeatureNotEnabledException
        //   PermissionException
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Problem while getting location", $"We had an error: {ex.Message}", "OK");
        }
        finally
        {
            IsCheckingLocation = false;
        }
    }

}
