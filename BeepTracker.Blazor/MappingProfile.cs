using AutoMapper;
using BeepTracker.Common.Models;
using BeepTracker.Common.Dtos;

namespace BeepTracker.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BeepRecord, BeepRecordDto>().ReverseMap();
            CreateMap<BeepEntry, BeepEntryDto>().ReverseMap();
            CreateMap<Bird, BirdDto>().ReverseMap();
            CreateMap<BirdStatus, BirdStatusDto>().ReverseMap();
        }
    }
}
