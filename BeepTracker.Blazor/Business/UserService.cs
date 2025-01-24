using AutoMapper;
using BeepTracker.Common.Models;
using BeepTracker.Common.Dtos;
using Microsoft.EntityFrameworkCore;
using static MudBlazor.CategoryTypes;


namespace BeepTracker.Blazor.Business
{

    public interface IUserService
    {
        IEnumerable<UserDto> GetAllForOrganisation(int organisationId);
        void Update(UserDto organisation);
        UserDto? GetById(int id);
    }

    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        public readonly BeepTrackerDbContext _context;

        public UserService(BeepTrackerDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<UserDto> GetAllForOrganisation(int organisationId)
        {
            return _context.Users.Where(b => b.OrganisationId == organisationId).Select(b => _mapper.Map<UserDto>(b));
        }

        public UserDto? GetById(int id)
        {
            var user = _context.Users.FirstOrDefault(b => b.Id == id);
            return _mapper.Map<UserDto>(user);
        }

        public void Update(UserDto user)
        {
            var userFromDatabase = _context.Users.FirstOrDefault(b => b.Id == user.Id);
            if (userFromDatabase != null)
            {
                _mapper.Map(user, userFromDatabase);
            }
            else
            {
                var updatedUser = _mapper.Map<User>(user);
                _context.Add(updatedUser);
            }
            _context.SaveChanges();
        }

    }
}
