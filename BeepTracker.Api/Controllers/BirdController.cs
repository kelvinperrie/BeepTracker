using AutoMapper;
using BeepTracker.Api.Dtos;
using BeepTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeepTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BirdController : ControllerBase
    {
        BeepTrackerDbContext _beepTrackerDbContext;
        IMapper _mapper;
        ILogger<BirdController> _logger;

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
                var birds = _beepTrackerDbContext.Birds;
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
