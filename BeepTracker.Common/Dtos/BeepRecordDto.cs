using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BeepTracker.Common.Dtos
{

    public class BeepRecordDto
    {
        public int Id { get; set; }
        
        public int BirdId { get; set; }
        
        public string ClientGeneratedKey { get; set; }

        public DateTime RecordedDateTime { get; set; }

        public string? BirdName { get; set; }

        public int BeatsPerMinute { get; set; }

        public string? Notes { get; set; }

        public string? Latitude { get; set; }

        public string? Longitude { get; set; }

        public string? Filename { get; set; }

        public int Status { get; set; }

        public virtual ICollection<BeepEntryDto> BeepEntries { get; set; } = new List<BeepEntryDto>();


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
