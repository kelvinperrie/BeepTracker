using Newtonsoft.Json;
using System.Windows.Input;

namespace BeepTracker.Maui.ViewModel;

[QueryProperty(nameof(BeepRecord), "BeepRecord")]
public partial class BeepEntryDetailsViewModel : BaseViewModel, INotifyPropertyChanged
{
    //public ObservableCollection<int> Beeps { get; } = new();
    public RelayCommand Add1Command { get; private set; }
    public RelayCommand Subtract1Command { get; private set; }
    public RelayCommand EnterNumberCommand { get; private set; }

    public ICommand ClearCommand { private set; get; }
    public ICommand BackspaceCommand { private set; get; }
    public ICommand DigitCommand { private set; get; }


    public event PropertyChangedEventHandler PropertyChanged;


    public int SelectedBeepEntryIndex;

    IMap map;
    public BeepEntryDetailsViewModel(IMap map)
    {
        this.map = map;

        SelectedBeepEntryIndex = 0;
        //Beeps[0].Selected = true;

        Add1Command = new RelayCommand(() => beepRecord.BeepEntries[SelectedBeepEntryIndex].Value++);
        Subtract1Command = new RelayCommand(() => beepRecord.BeepEntries[SelectedBeepEntryIndex].Value--);

        ClearCommand = new Command(
            execute: () =>
            {
                //Entry = "0";
                //RefreshCanExecutes();
            });
        DigitCommand = new Command<string>(
            execute: (string arg) =>
            {
                int converted = int.Parse(arg);
                beepRecord.BeepEntries[SelectedBeepEntryIndex].Value = converted;

                //Entry += arg;
                //if (Entry.StartsWith("0") && !Entry.StartsWith("0."))
                //{
                //    Entry = Entry.Substring(1);
                //}
                //RefreshCanExecutes();
            },
            canExecute: (string arg) =>
            {
                return true; // !(arg == "." && Entry.Contains("."));
            });
    }

    public BeepRecord BeepRecord
    {
        get
        {
            return beepRecord;
        }
        set
        {
            if(beepRecord == null && value !=null)
            {
                // this is being set as it is passed into the page, so default the first beep to being selected
                value.BeepEntries[0].Selected = true;
            }
            if (beepRecord != value)
            {
                beepRecord = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BeepRecord"));
            }
        }
    }

    //[ObservableProperty]
    BeepRecord beepRecord;

    [RelayCommand]
    public void SaveBeepRecord()
    {
        if (BeepRecord == null) return;
        if (BeepRecord.Filename == null)
        {
            BeepRecord.Filename = DateTime.Now.ToString("o").Replace(":", "");
        }

        var basePath = FileSystem.Current.AppDataDirectory;
        var path = Path.Combine(basePath, "beeprecords");
        var filePath = Path.Combine(path, BeepRecord.Filename);

        var jsonData = JsonConvert.SerializeObject(BeepRecord, Formatting.Indented);

        File.WriteAllText(filePath, jsonData);

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
