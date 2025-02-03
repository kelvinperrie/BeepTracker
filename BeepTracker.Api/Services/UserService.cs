using AutoMapper;
using BeepTracker.Api.Controllers;
using BeepTracker.Common.Models;

namespace BeepTracker.Api.Services
{
    public interface IUserService
    {
        bool IsValidUser(string username, string password);
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

        public bool IsValidUser(string username, string password)
        {
            _logger.LogDebug($"Request received to validate credentials for user {username}");

            var foundUser = _beepTrackerDbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (foundUser != null)
            {
                if (foundUser.Active)
                {
                    return true;
                }
                else
                {
                    _logger.LogWarning($"User {username} attempted to connect but is inactive; connection is being rejected.");
                }
            }
            else
            {
                _logger.LogWarning($"Unable to find a matching username and passowrd in database for {username}.");
            }


            return false;
        }
    }
}
