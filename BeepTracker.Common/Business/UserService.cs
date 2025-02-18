using AutoMapper;
using BeepTracker.Common.Models;
using BeepTracker.Common.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeepTracker.Common.Business
{

    public interface IUserService
    {
        IEnumerable<UserDto> GetAllForOrganisation(int organisationId);
        void Update(UserDto organisation);
        UserDto? GetById(int id);
        User? GetUserByUsernameAndPassword(string username, string password);
    }

    public class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        public readonly BeepTrackerDbContext _context;

        public UserService(BeepTrackerDbContext context, IMapper mapper, ILogger<UserService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
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

        public User? GetUserByUsernameAndPassword(string username, string password)
        {
            // todo we're going to have to hash this pw at some point
            var hashedPassword = password;
            var foundUser = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == hashedPassword);

            if (foundUser == null)
            {
                _logger.LogWarning($"No matching user and password found when getting for {username}");
                return null;
            }

            return foundUser;
        }

    }
}
