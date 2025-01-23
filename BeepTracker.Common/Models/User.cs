using System;
using System.Collections.Generic;

namespace BeepTracker.Common.Models;

public partial class User
{
    public int Id { get; set; }

    public int OrganisationId { get; set; }

    public string? Name { get; set; }

    public string Username { get; set; } = null!;

    public string? Password { get; set; }

    public bool Active { get; set; }

    public virtual Organisation Organisation { get; set; } = null!;
}
