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
    public class OrganisationController : ControllerBase
    {
        private readonly BeepTrackerDbContext _beepTrackerDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<BirdController> _logger;
        private readonly IOrganisationService _organisationService;

        public OrganisationController(BeepTrackerDbContext beepTrackerDbContext, IMapper mapper, ILogger<BirdController> logger, IOrganisationService organisationService) 
        {
            _beepTrackerDbContext = beepTrackerDbContext;
            _mapper = mapper;
            _logger = logger;
            _organisationService = organisationService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<OrganisationDto>> Get()
        {
            try
            {
                _logger.LogDebug("Request recieved to get organisation list");

                // who is requesting this? Get the organisations based on their user
                var userName = this.User.Identity?.Name;
                if (string.IsNullOrEmpty(userName)) {
                    throw new Exception("Unable to get user name, so unable to determine organisations to retrieve");
                }

                var organisations = _organisationService.GetAllByUsername(userName).ToList();

                _logger.LogDebug($"Returning {organisations.Count} organisations");
                return organisations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting organisation list");
                throw;
            }
        }

    }
}
