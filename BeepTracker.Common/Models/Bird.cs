﻿using System;
using System.Collections.Generic;

namespace BeepTracker.Common.Models;

public partial class Bird
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int OrganisationId { get; set; }

    public int StatusId { get; set; }

    public virtual ICollection<BeepRecord> BeepRecords { get; set; } = new List<BeepRecord>();

    public virtual Organisation Organisation { get; set; } = null!;

    public virtual BirdStatus Status { get; set; } = null!;
}
