﻿using System;
using System.Collections.Generic;

namespace BeepTracker.ApiClient.Models
{

    public partial class Bird
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<BeepRecord> BeepRecords { get; set; } = new List<BeepRecord>();
    }
}