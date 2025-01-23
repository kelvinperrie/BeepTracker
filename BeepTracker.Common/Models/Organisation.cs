using System;
using System.Collections.Generic;

namespace BeepTracker.Common.Models;

public partial class Organisation
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
