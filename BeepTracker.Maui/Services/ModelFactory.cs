using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.Maui.Services
{
    // todo use the one in common
    public class ModelFactory
    {
        public BeepRecord CreateBeepRecord()
        {
            var beepEntryCount = 16; // may be variable in the future?

            var beepRecord = new BeepRecord
            {
                ClientGeneratedKey = Guid.NewGuid().ToString(),
                BeepEntries = [],
                RecordedDate = DateTime.Now,
                RecordedTime = DateTime.Now.TimeOfDay,
                UploadStatus = (int)BeepRecordUploadStatus.Created
            };

            for (var i = 0; i < beepEntryCount; i++)
            {
                beepRecord.BeepEntries.Add(new BeepEntry() { Index = i });
            }

            return beepRecord;
        }
    }
}
