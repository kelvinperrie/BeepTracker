using Asp.Versioning;
using AutoMapper;
using BeepTracker.Api.Dtos;
using BeepTracker.Api.Lookups;
using BeepTracker.Api.Models;
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

        public BirdController(BeepTrackerDbContext beepTrackerDbContext, IMapper mapper, ILogger<BirdController> logger) 
        {
            _beepTrackerDbContext = beepTrackerDbContext;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<BirdDto>> Get()
        {
            // todo - will probably need to limit this based on active or status etc
            try
            {
                _logger.LogDebug("Request recieved to get bird list");

                // who is requesting this? Get the birds based on their organisation
                var userName = this.User.Identity?.Name;
                if (string.IsNullOrEmpty(userName)) {
                    throw new Exception("Unable to get user name, so unable to determine bird list to retrieve");
                }

                var user = _beepTrackerDbContext.Users.FirstOrDefault(u => u.Username == userName);
                if (user == null) {
                    throw new Exception($"Unable to find user with username of {userName} so can't locate their organisation");
                }

                var birds = _beepTrackerDbContext.Birds.Where(b => b.OrganisationId == user.OrganisationId && b.StatusId == (int)BirdStatusLookup.Active);
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
