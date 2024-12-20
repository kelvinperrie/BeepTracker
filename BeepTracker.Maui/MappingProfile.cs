using AutoMapper;
using BeepTracker.ApiClient;

namespace BeepTracker.Maui
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BeepRecord, ApiClient.Models.BeepRecord>().ReverseMap();
            CreateMap<BeepEntry, ApiClient.Models.BeepEntry>().ReverseMap();
        }
    }
}
