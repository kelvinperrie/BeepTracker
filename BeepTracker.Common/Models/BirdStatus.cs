using System;
using System.Collections.Generic;

namespace BeepTracker.Common.Models;

public partial class BirdStatus
{
    public int Id { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Bird> Birds { get; set; } = new List<Bird>();
}
