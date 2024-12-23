using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeepTracker.Maui.Model
{
    /// <summary>
    /// this class describes the beeprecord that is stored locally on the mobile device
    /// </summary>
    public partial class BeepRecord : ObservableObject
    {
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public string clientGeneratedKey;
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public DateTime recordedDateTime;
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public string birdName;
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public int birdId;
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
        public int uploadStatus;
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public string? syncResponse;

        /// <summary>
        /// this timespan is based on the datetime property so that we can link the onscreen time picker
        /// to it
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public TimeSpan RecordedTime
        {
            get
            {
                return RecordedDateTime.TimeOfDay;
            }
            set
            {
                RecordedDateTime = RecordedDateTime.Date.Add(value);
            }
        }
        /// <summary>
        /// this datetime exists so we can surface just the date value of the recordeddatetime to link
        /// the onscreen date picker to it
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public DateTime RecordedDate
        {
            get
            {
                return RecordedDateTime.Date;
            }
            set
            {
                RecordedDateTime = value.Add( RecordedDateTime.TimeOfDay);
            }
        }


        public BeepRecord()
        {

        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// used to compare to beeprecords together to determine if one has changed (i.e. "do you want to save" prompt)
        /// </summary>
        /// <param name="other">a beeprecord to compare this one against</param>
        /// <returns>true if the records have equivalent data</returns>
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
