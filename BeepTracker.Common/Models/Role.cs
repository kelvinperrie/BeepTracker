using System;
using System.Collections.Generic;

namespace BeepTracker.Common.Models;

public partial class Role
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<OrganisationUserRole> OrganisationUserRoles { get; set; } = new List<OrganisationUserRole>();
}
