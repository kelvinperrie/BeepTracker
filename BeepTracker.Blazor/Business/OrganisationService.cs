using AutoMapper;
using BeepTracker.Common.Dtos;
using BeepTracker.Common.Models;
using static MudBlazor.CategoryTypes;

namespace BeepTracker.Blazor.Business
{
    public interface IOrganisationService
    {
        IEnumerable<OrganisationDto> GetAll();
        void Update(OrganisationDto organisation);
        OrganisationDto? GetById(int id);
    }

    public class OrganisationService : IOrganisationService
    {

        private readonly IMapper _mapper;
        public readonly BeepTrackerDbContext _context;

        public OrganisationService(BeepTrackerDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<OrganisationDto> GetAll()
        {
            return _context.Organisations.Select(o => _mapper.Map<OrganisationDto>(o));
        }

        public OrganisationDto? GetById(int id)
        {
            var organisation = _context.Organisations.FirstOrDefault(o => o.Id == id);
            return organisation == null ? null : _mapper.Map<OrganisationDto>(organisation);
        }

        public void Update(OrganisationDto organisation)
        {

            var organisationInDatabase = _context.Organisations.FirstOrDefault(o => o.Id == organisation.Id);
            if (organisationInDatabase != null)
            {
                _mapper.Map(organisation, organisationInDatabase);
            }
            else
            {
                var updatedOrganisation = _mapper.Map<Organisation>(organisation);
                _context.Add(updatedOrganisation);
            }
            _context.SaveChanges();
        }

    }
}
