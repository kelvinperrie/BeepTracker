using System;
using System.Collections.Generic;

namespace BeepTracker.Api.Models;

public partial class Organisation
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
