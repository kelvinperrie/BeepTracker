using AutoMapper;
using BeepTracker.Api.Controllers;
using BeepTracker.Common.Models;

namespace BeepTracker.Api.Services
{
    public interface IUserService
    {
        User? GetUserByUsernameAndPassword(string username, string password);
    }

    public class UserService : IUserService
    {
        private readonly BeepTrackerDbContext _beepTrackerDbContext;
        private readonly ILogger<UserService> _logger;

        public UserService(BeepTrackerDbContext beepTrackerDbContext, ILogger<UserService> logger)
        {
            _beepTrackerDbContext = beepTrackerDbContext;
            _logger = logger;
        }

        public User? GetUserByUsernameAndPassword(string username, string password)
        {
            var foundUser = _beepTrackerDbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if(foundUser == null)
            {
                _logger.LogWarning($"No matching user and password found when getting for {username}");
                return null;
            }

            return foundUser;
        }

    }
}
