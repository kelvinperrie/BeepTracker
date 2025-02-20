using Asp.Versioning;
using AutoMapper;
using BeepTracker.Api.Lookups;
using BeepTracker.Common.Business;
using BeepTracker.Common.Dtos;
using BeepTracker.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeepTracker.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class BirdController : ControllerBase
    {
        private readonly BeepTrackerDbContext _beepTrackerDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<BirdController> _logger;
        private readonly IUserService _userService;

        public BirdController(BeepTrackerDbContext beepTrackerDbContext, IMapper mapper, ILogger<BirdController> logger, IUserService userService) 
        {
            _beepTrackerDbContext = beepTrackerDbContext;
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("Get/{organisationId}")]
        public ActionResult<IEnumerable<BirdDto>> GetForOrganisation(int organisationId)
        {
            try
            {
                _logger.LogDebug($"Request recieved to get bird list for organisation {organisationId}");

                // confirm that the user requesting this, is a member of the organisation (e.g. is allowed to get the list)
                // who is requesting this? Get the birds based on their organisation
                var userName = this.User.Identity?.Name;
                if (string.IsNullOrEmpty(userName))
                {
                    throw new Exception("Unable to get user name, so unable to determine bird list to retrieve");
                }

                var user = _userService.GetByUsername(userName);
                if (user == null)
                {
                    throw new Exception($"Unable to find user with username of {userName} so can't locate their organisation");
                }

                var roleForOrg = user.OrganisationUserRoles.FirstOrDefault(our => our.OrganisationId == organisationId);
                if(roleForOrg == null)
                {
                    throw new Exception($"User {userName} is not a member of organisation with id of {organisationId} so is not allowed to get a list of their birds");
                }

                var birds = _beepTrackerDbContext.Birds.Where(b => b.OrganisationId == organisationId && b.StatusId == (int)BirdStatusLookup.Active);
                var birdDtos = _mapper.Map<List<BirdDto>>(birds);
                _logger.LogDebug($"Returning {birdDtos.Count} birds");
                return birdDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting bird list");
                throw;
            }
        }

    }
}
