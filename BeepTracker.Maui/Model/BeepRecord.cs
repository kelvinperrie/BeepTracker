using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeepTracker.Maui.Model
{
    public partial class BeepRecord : ObservableObject
    {
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public DateTime recordedTime;
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public string birdName;
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public List<BeepEntry> beepEntries;
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public string notes;
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public string latitude;
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public string longitude;
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public string filename;

        public BeepRecord()
        {
            if(beepEntries == null)
            {
                beepEntries = new List<BeepEntry>();
                beepEntries.Add(new BeepEntry() { Value = 0 });
                beepEntries.Add(new BeepEntry() { Value = 0 });
                beepEntries.Add(new BeepEntry() { Value = 0 });
                beepEntries.Add(new BeepEntry() { Value = 0 });
                beepEntries.Add(new BeepEntry() { Value = 0 });
                beepEntries.Add(new BeepEntry() { Value = 0 });
                beepEntries.Add(new BeepEntry() { Value = 0 });
                beepEntries.Add(new BeepEntry() { Value = 0 });
                beepEntries.Add(new BeepEntry() { Value = 0 });
                beepEntries.Add(new BeepEntry() { Value = 0 });
                beepEntries.Add(new BeepEntry() { Value = 0 });
                beepEntries.Add(new BeepEntry() { Value = 0 });
                beepEntries.Add(new BeepEntry() { Value = 0 });
                beepEntries.Add(new BeepEntry() { Value = 0 });
                beepEntries.Add(new BeepEntry() { Value = 0 });
            }
        }
    }

    [JsonSerializable(typeof(List<BeepRecord>))]
    internal sealed partial class BeepRecordContext : JsonSerializerContext
    {

    }
}
