using System;
using System.Collections.Generic;

namespace BeepTracker.Common.Dtos;

public partial class UserDto
{
    public int Id { get; set; }

    public int OrganisationId { get; set; }

    public string Name { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool Active { get; set; }

}
