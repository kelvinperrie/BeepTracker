using System;
using System.Collections.Generic;

namespace BeepTracker.Api.Dtos;

public class BirdDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<BeepRecordDto> BeepRecords { get; set; } = new List<BeepRecordDto>();
}
