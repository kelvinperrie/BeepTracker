using System;
using System.Collections.Generic;

namespace BeepTracker.Api.Models;

public partial class Bird
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int OrganisationId { get; set; }

    public int StatusId { get; set; }

    public virtual ICollection<BeepRecord> BeepRecords { get; set; } = new List<BeepRecord>();
}
