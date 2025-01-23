using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BeepTracker.Blazor.Models;

[MetadataType(typeof(BirdStatusMetaData))]
[ModelMetadataType(typeof(BirdStatusMetaData))]
public partial class BirdStatus
{
    public int Id { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Bird> Birds { get; set; } = new List<Bird>();
}


public partial class BirdStatusMetaData
{
    [JsonIgnore]
    public virtual ICollection<Bird> Birds { get; set; } = new List<Bird>();
}

