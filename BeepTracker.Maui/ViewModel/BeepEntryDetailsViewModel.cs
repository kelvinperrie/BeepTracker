﻿using BeepTracker.Maui.Services;
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

    IMap map;
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
                int converted = int.Parse(arg);
                BeepRecord.BeepEntries[SelectedBeepEntryIndex].Value = converted;
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

    public BeepRecord BeepRecord
    {
        get
        {
            return beepRecord;
        }
        set
        {
            //if(beepRecord == null && value !=null)
            //{
            //    // this is being set as it is passed into the page, so default the first beep to being selected
            //    value.BeepEntries[0].Selected = true;
            //}
            if (beepRecord != value)
            {
                if (beepRecord == null && value != null)
                {
                    if (value.Filename == null)
                    {
                        // this is a new file ... ?
                        beepRecord = value;
                        beepRecord.BeepEntries[0].Selected = true;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BeepRecord"));
                        return;
                    }
                    // this is a bit mickey mouse, for some reason changes made on the details page are being persisted
                    // on the list page, even when they're not saved. To get around this when the details page loads 
                    // we're going to load the beeprecord from file rather than using the passed through item
                    // todo I should figure out why it is doing this
                    var recordFromFile = localPersistance.GetBeepRecordByFilename(value.Filename);
                    beepRecord = recordFromFile;
                    // this is being set as it is passed into the page, so default the first beep to being selected
                    beepRecord.BeepEntries[0].Selected = true;
                }
                else
                {
                    beepRecord = value;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BeepRecord"));
            }
        }
    }

    //[ObservableProperty]
    BeepRecord beepRecord;


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
                await Shell.Current.GoToAsync("//MainPage", true);
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
                await Shell.Current.GoToAsync("//MainPage", true);
            } else
            {
                // save the data? or just let them push the save button???
            }
        } else
        {
            // no changes
            await Shell.Current.GoToAsync("//MainPage", true);
        }
    }

    [RelayCommand]
    public void NextBeepEntry()
    {
        if (SelectedBeepEntryIndex == beepRecord.BeepEntries.Count - 1) return;
        beepRecord.BeepEntries[SelectedBeepEntryIndex].Selected = false;
        SelectedBeepEntryIndex++;
        beepRecord.BeepEntries[SelectedBeepEntryIndex].Selected = true;
    }
    [RelayCommand]
    public void PreviousBeepEntry()
    {
        if (SelectedBeepEntryIndex == 0) return;
        beepRecord.BeepEntries[SelectedBeepEntryIndex].Selected = false;
        SelectedBeepEntryIndex--;
        beepRecord.BeepEntries[SelectedBeepEntryIndex].Selected = true;
    }

    //[RelayCommand]
    //async Task OpenMap()
    //{
    //    try
    //    {
    //        await map.OpenAsync(beepRecord.Latitude, MobeepRecordnkey.Longitude, new MapLaunchOptions
    //        {
    //            Name = beepRecord.BirdName,
    //            NavigationMode = NavigationMode.None
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($"Unable to launch maps: {ex.Message}");
    //        await Shell.Current.DisplayAlert("Error, no Maps app!", ex.Message, "OK");
    //    }
    //}
}
