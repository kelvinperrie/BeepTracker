using AutoMapper;
using BeepTracker.Common.Dtos;
using BeepTracker.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace BeepTracker.Common.Business
{
    public interface IOrganisationService
    {
        IEnumerable<OrganisationDto> GetAll();
        void Update(OrganisationDto organisation);
        OrganisationDto? GetById(int id);
        IEnumerable<OrganisationDto> GetAllByUsername(string username);
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

        public IEnumerable<OrganisationDto> GetAllByUsername(string username)
        {
            var organisations = _context.Users.Include(u => u.OrganisationUserRoles).ThenInclude(our => our.Organisation)
                .First(u => u.Username == username)
                .OrganisationUserRoles.Where(our => our.Active)
                .Select(ur => ur.Organisation);

            var organisationDtos = organisations.Select(o => _mapper.Map<OrganisationDto>(o));
            return organisationDtos;

            //var organisations = _context.Organisations.Where(o => o.OrganisationUserRoles)
            //return _context.Organisations.Select(o => _mapper.Map<OrganisationDto>(o));
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
