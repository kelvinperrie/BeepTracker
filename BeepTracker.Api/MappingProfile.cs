using AutoMapper;
using Azure;
using BeepTracker.Common.Dtos;
using BeepTracker.Common.Models;

namespace BeepTracker.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BeepRecord, BeepRecordDto>().ReverseMap();
            CreateMap<BeepEntry, BeepEntryDto>().ReverseMap();
            CreateMap<Bird, BirdDto>().ReverseMap();
            CreateMap<Organisation, OrganisationDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<OrganisationUserRole, OrganisationUserRoleDto>().ReverseMap();
        }
    }
}
