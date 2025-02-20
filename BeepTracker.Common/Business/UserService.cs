using AutoMapper;
using BeepTracker.Common.Models;
using BeepTracker.Common.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BeepTracker.Common.Extensions;

namespace BeepTracker.Common.Business
{

    public interface IUserService
    {
        IEnumerable<OrganisationUserDto> GetAllForOrganisation(int organisationId);
        void Update(UserDto organisation);
        void Update(OrganisationUserDto user, int organisationId);
        UserDto? GetById(int id);
        UserDto? GetByUsername(string username);
        User? GetUserByUsernameAndPassword(string username, string password);
        void SetPassword(int userId, string password);

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

        public IEnumerable<OrganisationUserDto> GetAllForOrganisation(int organisationId)
        {

            var users = _context.OrganisationUserRoles
                .Include(our => our.User).ThenInclude(u => u.OrganisationUserRoles).ThenInclude(our => our.Role)
                .Where(our => our.OrganisationId == organisationId).Select(our => our.User);

            return users.Select(u => u.MapTo(organisationId, _mapper));


            //var users = _context.Users
            //    .Include(u => u.OrganisationUserRoles).ThenInclude(our => our.Role)
            //    .Where(b => b.OrganisationId == organisationId).Select(b => _mapper.Map<UserDto>(b));

            //return users;
        }

        public UserDto? GetById(int id)
        {
            var user = _context.Users.FirstOrDefault(b => b.Id == id);
            return _mapper.Map<UserDto>(user);
        }
        public UserDto? GetByUsername(string username)
        {
            var user = _context.Users
                .Include(u => u.OrganisationUserRoles)
                .ThenInclude(u => u.Role)
                .FirstOrDefault(b => b.Username == username);
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

        public void Update(OrganisationUserDto user, int organisationId)
        {
            var userFromDatabase = _context.Users.FirstOrDefault(b => b.Id == user.Id);

            if (userFromDatabase == null)
            {
                // couldn't get the user by id, because they are a new user (so id = 0)
                // check to see if there is a user in the db with the same username, if so we want to use that as the user (i.e. link the existing user to this organisation)
                userFromDatabase = _context.Users.FirstOrDefault(u => u.Username == user.Username);
                if(userFromDatabase == null)
                {
                    // we couldn't find this user id in the db, so we need to create this user from scratch
                    var newUser = _mapper.Map<User>(user);
                    newUser.Password = Guid.NewGuid().ToString(); // cram something into the password
                    newUser.OrganisationUserRoles.Add(new OrganisationUserRole
                    {
                        OrganisationId = organisationId,
                        RoleId = user.Role.Id,
                        Active = user.Active
                    });

                    _context.Users.Add(newUser);
                }
                else
                {
                    // the user exists but is not part of the current organisation, so add them to this organisation with this role
                    // we lose the user id when we map, so remember it then restore it
                    var userId = userFromDatabase.Id;
                    _mapper.Map(user, userFromDatabase);
                    userFromDatabase.Id = userId;
                    userFromDatabase.OrganisationUserRoles.Add(new OrganisationUserRole
                    {
                        OrganisationId = organisationId,
                        RoleId = user.Role.Id,
                        Active = user.Active
                    });
                }

            }
            else
            {
                // we got the user so we know we are editing an existing user on this organisation
                var roleLink = userFromDatabase.OrganisationUserRoles.FirstOrDefault(our => our.OrganisationId == organisationId);
                if (roleLink != null)
                {
                    // user is part of the organisation, so update their role and details
                    _mapper.Map(user, userFromDatabase);
                    roleLink.RoleId = user.Role.Id;
                    roleLink.Active = user.Active;
                }
                else
                {
                    throw new Exception($"unable to get the role link for give user with id of {user.Id} for organisation {organisationId}");
                }
            }
            _context.SaveChanges();
        }

        public void SetPassword(int userId, string password)
        {
            // todo hash etc
            var userToUpdate = _context.Users.First(u => u.Id == userId);
            userToUpdate.Password = password;
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
