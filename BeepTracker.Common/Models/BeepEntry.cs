using System;
using System.Collections.Generic;

namespace BeepTracker.Common.Models;

public partial class BeepEntry
{
    public int Id { get; set; }

    public int BeepRecordId { get; set; }

    public int Value { get; set; }

    public int? Index { get; set; }

    public virtual BeepRecord BeepRecord { get; set; } = null!;
}
