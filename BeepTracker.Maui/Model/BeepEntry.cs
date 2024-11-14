using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeepTracker.Maui.Model
{
    [MetadataType(typeof(BeepEntryMetadata))]
    public partial class BeepEntry : ObservableObject
    {
        public BeepEntry()
        {
            Value = null;
            Selected = false;
        }

        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public int? value;      // the amount of beeps recorded
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public bool selected;   // indicates if it is the beep entry currently being updated
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public int index;       // which beep in the collection is this

        // indicates this beep entry is the first in a couplet
        public bool IsFirstItemInCouplet { get { return Index % 2 == 0; } }
    }

    public class BeepEntryMetadata
    {
        [Newtonsoft.Json.JsonIgnore]
        public bool Selected { get; set; }
    }

    [JsonSerializable(typeof(List<BeepEntry>))]
    internal sealed partial class BeepEntryContext : JsonSerializerContext
    {

    }
}
