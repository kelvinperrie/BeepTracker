using AutoMapper;
using BeepTracker.Api.Dtos;
using BeepTracker.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeepTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeepRecordController : ControllerBase
    {
        private readonly BeepTrackerDbContext _beepTrackerDbContext;
        private readonly IMapper _mapper;

        public BeepRecordController(BeepTrackerDbContext beepTrackerDbContext, IMapper mapper) 
        {
            _beepTrackerDbContext = beepTrackerDbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<BeepRecordDto>> Get()
        {
            var records = _beepTrackerDbContext.BeepRecords.Include(b => b.BeepEntries);
            var recordDtos = _mapper.Map<List<BeepRecordDto>>(records);
            return recordDtos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BeepRecordDto>> GetById(int id)
        {
            var record = await _beepTrackerDbContext.BeepRecords.Where(b => b.Id == id).Include(b => b.BeepEntries).SingleOrDefaultAsync();
            if (record is null)
            {
                return NotFound();
            }
            var recordDto = _mapper.Map<BeepRecordDto>(record);
            return recordDto;
        }
        [HttpGet("GetByClientGeneratedKey/{key}")]
        public async Task<ActionResult<BeepRecordDto>> GetByClientGeneratedKey(string key)
        {
            var record = await _beepTrackerDbContext.BeepRecords.Where(b => b.ClientGeneratedKey == key).Include(b => b.BeepEntries).SingleOrDefaultAsync();
            if (record is null)
            {
                return null;
            }
            var recordDto = _mapper.Map<BeepRecordDto>(record);
            return recordDto;
        }

        [HttpPost]
        public async Task<ActionResult> Create(BeepRecordDto beepRecord)
        {
            var record = _mapper.Map<BeepRecord>(beepRecord);
            await _beepTrackerDbContext.BeepRecords.AddAsync(record);
            await _beepTrackerDbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = beepRecord.Id }, beepRecord);
        }

        [HttpPut]
        public async Task<ActionResult> Update(BeepRecordDto beepRecord)
        {
            var record = _mapper.Map<BeepRecord>(beepRecord);
            // this does not automatically remove/update beepentry child items, so every save we end up with duplicate beepentry records
            // they get duplicated because we don't have an index with them on the device - so they come in with index = 0
            // which means they get saved as new entries
            // we'll manaully remove any existing ones
            var beepEntriesInDatabase = _beepTrackerDbContext.BeepEntries.Where(b => b.BeepRecordId == beepRecord.Id);
            foreach(var beepEntry in beepEntriesInDatabase)
            {
                _beepTrackerDbContext.Remove(beepEntry);
            }
            _beepTrackerDbContext.BeepRecords.Update(record);
            await _beepTrackerDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var item = await GetById(id);
            if (item is null)
            {
                return NotFound();
            }
            _beepTrackerDbContext.Remove(item);
            await _beepTrackerDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
