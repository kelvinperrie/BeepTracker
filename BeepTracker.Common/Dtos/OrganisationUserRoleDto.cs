using BeepTracker.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.Common.Dtos
{
    public class OrganisationUserRoleDto
    {
        public int UserId { get; set; }

        public int OrganisationId { get; set; }

        public int RoleId { get; set; }

        public bool Active { get; set; }

        public RoleDto Role { get; set; }
    }
}
