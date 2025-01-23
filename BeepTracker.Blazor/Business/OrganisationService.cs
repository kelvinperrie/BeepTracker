using BeepTracker.Common.Models;
using static MudBlazor.CategoryTypes;

namespace BeepTracker.Blazor.Business
{
    public interface IOrganisationService
    {
        IEnumerable<Organisation> GetAll();
        void Update(Organisation organisation);
    }

    public class OrganisationService : IOrganisationService
    {

        public readonly BeepTrackerDbContext _context;

        public OrganisationService(BeepTrackerDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Organisation> GetAll()
        {
            return _context.Organisations;
        }

        public void Update(Organisation organisation)
        {
            if (_context.Organisations.Any(w => w.Id == organisation.Id))
            {
                _context.Attach(organisation);
            }
            else
            {
                _context.Add(organisation);
            }
            _context.SaveChanges();
        }

    }
}
