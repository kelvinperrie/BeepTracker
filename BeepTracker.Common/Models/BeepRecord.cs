﻿using System;
using System.Collections.Generic;

namespace BeepTracker.Common.Models;

public partial class BeepRecord
{
    public int Id { get; set; }

    public int BirdId { get; set; }

    public string ClientGeneratedKey { get; set; } = null!;

    public DateTime RecordedDateTime { get; set; }

    public string? BirdName { get; set; }

    public int BeatsPerMinute { get; set; }

    public string? Notes { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public string? Filename { get; set; }

    public long Status { get; set; }

    public int? UserId { get; set; }

    public DateTime? DateSaved { get; set; }

    public virtual ICollection<BeepEntry> BeepEntries { get; set; } = new List<BeepEntry>();

    public virtual Bird Bird { get; set; } = null!;

    public virtual User? User { get; set; }
}
