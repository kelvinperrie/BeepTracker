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
        public int? value;
        [ObservableProperty]
        [Newtonsoft.Json.JsonIgnore]
        public bool selected;
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
