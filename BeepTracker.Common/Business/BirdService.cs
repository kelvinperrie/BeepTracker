using AutoMapper;
using BeepTracker.Common.Models;
using BeepTracker.Common.Dtos;
using Microsoft.EntityFrameworkCore;

namespace BeepTracker.Common.Business
{

    public interface IBirdService
    {
        IEnumerable<BirdDto> GetAllForOrganisation(int organisationId);
        void Update(BirdDto organisation);
        BirdDto? GetById(int id);
    }

    public class BirdService : IBirdService
    {
        private readonly IMapper _mapper;
        public readonly BeepTrackerDbContext _context;

        public BirdService(BeepTrackerDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<BirdDto> GetAllForOrganisation(int organisationId)
        {
            return _context.Birds.Include(b => b.Status).Where(b => b.OrganisationId == organisationId).Select(b => _mapper.Map<BirdDto>(b));
        }

        public BirdDto? GetById(int id)
        {
            var bird = _context.Birds.FirstOrDefault(b => b.Id == id);
            return _mapper.Map<BirdDto>(bird);
        }

        public void Update(BirdDto bird)
        {
            var updatedBird = _mapper.Map<Bird>(bird);
            if (_context.Birds.Any(w => w.Id == updatedBird.Id))
            {
                var birdToUpdate = _context.Birds.First(b => b.Id == updatedBird.Id);
                _mapper.Map(bird, birdToUpdate);

            }
            else
            {
                _context.Add(updatedBird);
            }
            _context.SaveChanges();
        }

    }
}
