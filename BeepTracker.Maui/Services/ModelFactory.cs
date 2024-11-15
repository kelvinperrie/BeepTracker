﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.Maui.Services
{
    public class ModelFactory
    {
        public BeepRecord CreateBeepRecord()
        {
            var beepEntryCount = 16;

            var beepRecord = new BeepRecord
            {
                BeepEntries = [],
                RecordedDate = DateTime.Now,
                RecordedTime = DateTime.Now.TimeOfDay
            };

            for (var i = 0; i < beepEntryCount; i++)
            {
                beepRecord.BeepEntries.Add(new BeepEntry() { Index = i });
            }

            return beepRecord;
        }
    }
}
