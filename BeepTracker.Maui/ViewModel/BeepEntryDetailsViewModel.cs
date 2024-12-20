﻿using BeepTracker.Maui.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Newtonsoft.Json;
using System.Windows.Input;

namespace BeepTracker.Maui.ViewModel;

[QueryProperty(nameof(BeepRecord), "BeepRecordPassed")]
public partial class BeepEntryDetailsViewModel : BaseViewModel, INotifyPropertyChanged
{
    // during page setup a reference of the page is passed to the viewmodel and stored here
    // this lets us reference controls on the page to do things like scroll to certain elements
    // ... there is probably a better way to do this, but I winged it
    public DetailsPage MyPage { get; set; }             


    public RelayCommand EnterNumberCommand { get; private set; }
    public ICommand BeatsPerMinuteClickedCommand { get; private set; }
    public ICommand BeepRecordStatusClickedCommand { get; private set; }
    public ICommand ClearCommand { private set; get; }
    public ICommand BackspaceCommand { private set; get; }
    public ICommand DigitCommand { private set; get; }

    private readonly ISettingsService settingsService;
    private readonly LocalPersistance localPersistance;

    public event PropertyChangedEventHandler PropertyChanged;


    public ObservableCollection<BeepTracker.ApiClient.Models.Bird> BirdsFromDatabase { get; } = new();
    [ObservableProperty]
    private int birdFromDatabaseIndex = -1;

    [ObservableProperty]
    public bool syncToDatabaseActive = false;   // indicates the app is set to try to sync records to the db - changes what elements are shown

    public int SelectedBeepEntryIndex;      // tracks which of the beep entries is currently selected
    public bool UserHasEnteredDigitIntoSelectedBeepEntry = false;   // tracks if the user has entered a digit into an entry after entering the field

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
                // based on the name property in the beeprecord, set the item to be selected in the bird dropdown
                var indexOfCurrentBirdName = BirdsFromDatabase.ToList().FindIndex(b => b.Name == beepRecord.BirdName);
                BirdFromDatabaseIndex = indexOfCurrentBirdName;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BirdFromDatabaseIndex"));
                // mark the first beep entry as being the selected one
                beepRecord.BeepEntries.ForEach(be => be.Selected = false);  // this is needed for when a record is updated and then reopened from list page
                beepRecord.BeepEntries[0].Selected = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BeepRecord"));
            }
        }
    }

    public BeepEntryDetailsViewModel( LocalPersistance localPersistance, ISettingsService settingsService)
    {
        this.localPersistance = localPersistance;
        this.settingsService = settingsService;

        foreach(var bird in this.settingsService.BirdListFromDatabase.OrderBy(b => b.Name))
        {
            BirdsFromDatabase.Add(bird);
        }

        SelectedBeepEntryIndex = 0;     // default to the first entry being selected on page load

        SyncToDatabaseActive = this.settingsService.AttemptToSyncRecords;

        BeepRecordStatusClickedCommand = new Command<string>(
            execute: (string arg) =>
            {
                int converted = int.Parse(arg);
                BeepRecord.UploadStatus = converted;
            },
            canExecute: (string arg) => { return true; });

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

    /// <summary>
    /// triggered when the bird selection drop down is changed
    /// </summary>
    /// <param name="value"></param>
    partial void OnBirdFromDatabaseIndexChanged(int value) {
        if (value != -1)
        {
            BeepRecord.BirdName = BirdsFromDatabase.ElementAt(value).Name;
            BeepRecord.BirdId = BirdsFromDatabase.ElementAt(value).Id;
        }
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
                await Shell.Current.GoToAsync("//" + nameof(MainPage), true);
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
                await Shell.Current.GoToAsync("//" + nameof(MainPage), true);
            } else
            {
                // save the data? or just let them push the save button???
            }
        } else
        {
            // no changes
            await Shell.Current.GoToAsync("//" + nameof(MainPage), true);
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
        // scroll horizontally so that the beep entries are always in view
        MyPage.ScrollToBeepEntry(beepRecord.BeepEntries[SelectedBeepEntryIndex]);
    }

    [RelayCommand]
    public void PreviousBeepEntry()
    {
        if (SelectedBeepEntryIndex == 0) return;
        beepRecord.BeepEntries[SelectedBeepEntryIndex].Selected = false;
        SelectedBeepEntryIndex--;
        beepRecord.BeepEntries[SelectedBeepEntryIndex].Selected = true;
        UserHasEnteredDigitIntoSelectedBeepEntry = false;
        // scroll horizontally so that the beep entries are always in view
        MyPage.ScrollToBeepEntry(beepRecord.BeepEntries[SelectedBeepEntryIndex]);
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

    [RelayCommand]
    public async Task DeleteBeepRecord()
    {
        try
        {
            bool answer = await Shell.Current.DisplayAlert("Delete Record?", "Are you sure you want to delete this record? There is no way to undo this.", "Yes", "No");

            if (answer)
            {
                localPersistance.DeleteBeepRecord(BeepRecord);
                await Shell.Current.GoToAsync("//" + nameof(MainPage), true);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Problem while deleting the beep record", $"We had an error: {ex.Message}", "OK");
        }
        finally
        {
        }
    }

}
