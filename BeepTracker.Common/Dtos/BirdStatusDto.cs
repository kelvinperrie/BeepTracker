using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BeepTracker.Common.Dtos;

public partial class BirdStatusDto
{
    public int Id { get; set; }

    public string? Status { get; set; }

}



