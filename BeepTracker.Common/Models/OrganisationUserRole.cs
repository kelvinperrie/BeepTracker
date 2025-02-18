using System;
using System.Collections.Generic;

namespace BeepTracker.Common.Models;

public partial class OrganisationUserRole
{
    public int UserId { get; set; }

    public int OrganisationId { get; set; }

    public int RoleId { get; set; }

    public bool Active { get; set; }

    public virtual Organisation Organisation { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
