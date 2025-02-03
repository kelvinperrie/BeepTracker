using AutoMapper;
using Azure;
using BeepTracker.Api.Dtos;
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
        }
    }
}
