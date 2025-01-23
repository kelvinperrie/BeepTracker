using System;
using System.Collections.Generic;

namespace BeepTracker.Common.Dtos
{

    public class BeepEntryDto
    {
        public int Id { get; set; }

        public int BeepRecordId { get; set; }

        public int Value { get; set; }

        public int? Index { get; set; }

    }
}