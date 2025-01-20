using System;
using System.Collections.Generic;

namespace BeepTracker.Api.Models;

public partial class BirdStatus
{
    public int Id { get; set; }

    public string? Status { get; set; }
}
