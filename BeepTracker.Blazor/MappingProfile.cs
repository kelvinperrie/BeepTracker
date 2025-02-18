using AutoMapper;
using BeepTracker.Common.Models;
using BeepTracker.Common.Dtos;

namespace BeepTracker.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BeepRecord, BeepRecordDto>().ReverseMap();
            CreateMap<BeepEntry, BeepEntryDto>().ReverseMap();
            CreateMap<Bird, BirdDto>().ReverseMap();
            CreateMap<BirdStatus, BirdStatusDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Organisation, OrganisationDto>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<OrganisationUserRole, OrganisationUserRoleDto>().ReverseMap();
        }
    }
}
