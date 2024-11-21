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
        public DateTime recordedDate;
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public TimeSpan recordedTime;
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public string birdName;
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public int? beatsPerMinute;
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
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public int status;

        [Newtonsoft.Json.JsonIgnore]
        public DateTime RecordedDateTime { 
            get
            {
                return RecordedDate.Date.Add(RecordedTime);
            } 
        }

        public BeepRecord()
        {

        }


        public bool ContentEquals(BeepRecord other)
        {
            if(other == null) return false;
            foreach (var prop in typeof(BeepRecord).GetProperties())
            {
                if (prop.Name == "BeepEntries")
                {
                    if (this.BeepEntries.Count != other.BeepEntries.Count) return false;
                    for(var i = 0; i < this.BeepEntries.Count; i++)
                    {
                        if (BeepEntries[i].Value != other.BeepEntries[i].Value) return false;
                    }
                }
                else
                {
                    var value1 = prop.GetValue(this);
                    var value2 = prop.GetValue(other);
                    if (value1 == null && value2 != null)
                    {
                        return false;
                    }
                    else if (value1 == null && value2 == null)
                    {

                    } else if (!value1.Equals(value2))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    [JsonSerializable(typeof(List<BeepRecord>))]
    internal sealed partial class BeepRecordContext : JsonSerializerContext
    {

    }
}
