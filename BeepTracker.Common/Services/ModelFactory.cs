using BeepTracker.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.Common.Services
{
    public class ModelFactory
    {
        public BeepRecord CreateBeepRecord()
        {
            var beepEntryCount = 16; // may be variable in the future?

            var beepRecord = new BeepRecord
            {
                ClientGeneratedKey = Guid.NewGuid().ToString(),
                BeepEntries = [],
                RecordedDateTime = DateTime.Now,
            };

            for (var i = 0; i < beepEntryCount; i++)
            {
                beepRecord.BeepEntries.Add(new BeepEntry() { Index = i });
            }

            return beepRecord;
        }
    }
}
