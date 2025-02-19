using AutoMapper;
using BeepTracker.Common.Dtos;
using BeepTracker.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.Common.Extensions
{
    public static class Mapping
    {

        public static OrganisationUserDto MapTo(this User user, int organisationId, IMapper mapper)
        {
            var roleForThisOrganisation = user.OrganisationUserRoles.FirstOrDefault(our => our.OrganisationId  == organisationId);

            //if (roleForThisOrganisation == null) 
            //{
            //    return null;
            //}

            var dto = new OrganisationUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
                Active = roleForThisOrganisation.Active,
                OrganisationId = roleForThisOrganisation.OrganisationId,
                Role = mapper.Map<RoleDto>(roleForThisOrganisation.Role)
            };

            return dto;
        }

    }
}
