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

        public BirdController(BeepTrackerDbContext beepTrackerDbContext, IMapper mapper) 
        {
            _beepTrackerDbContext = beepTrackerDbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<BirdDto>> Get()
        {
            // todo active? based on status?
            var birds = _beepTrackerDbContext.Birds;
            return _mapper.Map<List<BirdDto>>(birds);
        }

    }
}
