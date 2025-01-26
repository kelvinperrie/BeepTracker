using System;
using System.Collections.Generic;

namespace BeepTracker.Common.Dtos;

public class BirdDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int OrganisationId { get; set; }

    public int StatusId { get; set; }

    public BirdStatusDto Status { get; set; }

    public virtual ICollection<BeepRecordDto> BeepRecords { get; set; } = new List<BeepRecordDto>();
}
