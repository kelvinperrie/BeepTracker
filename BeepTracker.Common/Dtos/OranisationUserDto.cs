using System;
using System.Collections.Generic;

namespace BeepTracker.Common.Dtos;

public partial class OrganisationUserDto
{
    // todo this is kind of a view model ... figure out where to put it
    public int Id { get; set; }

    public int OrganisationId { get; set; }

    public string Name { get; set; } = null!;

    public string Username { get; set; } = null!;

    public bool Active { get; set; }

    public virtual RoleDto Role { get; set; }

}
