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
            try
            {
                // todo - will probably need to limit this based on active or status etc
                _logger.LogInformation("Request recieved to get bird list");
                var birds = _beepTrackerDbContext.Birds;
                return _mapper.Map<List<BirdDto>>(birds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting bird list");
                throw;
            }
        }

    }
}
