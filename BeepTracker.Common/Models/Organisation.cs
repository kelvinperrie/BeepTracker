﻿using System;
using System.Collections.Generic;

namespace BeepTracker.Common.Models;

public partial class Organisation
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Bird> Birds { get; set; } = new List<Bird>();

    public virtual ICollection<OrganisationUserRole> OrganisationUserRoles { get; set; } = new List<OrganisationUserRole>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
