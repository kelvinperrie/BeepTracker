using AutoMapper;
using BeepTracker.Api.Dtos;
using BeepTracker.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BeepTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BeepRecordController : ControllerBase
    {
        private readonly BeepTrackerDbContext _beepTrackerDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<BeepRecordController> _logger;

        public BeepRecordController(BeepTrackerDbContext beepTrackerDbContext, IMapper mapper,
            ILogger<BeepRecordController> logger) 
        {
            _beepTrackerDbContext = beepTrackerDbContext;
            _mapper = mapper;
            _logger = logger;
        }

        // unsure if to be used, included for now for sake of CRUD completeness
        [HttpGet]
        public ActionResult<IEnumerable<BeepRecordDto>> Get()
        {
            var records = _beepTrackerDbContext.BeepRecords.Include(b => b.BeepEntries);
            var recordDtos = _mapper.Map<List<BeepRecordDto>>(records);
            return recordDtos;
        }

        // unsure if to be used, included for now for sake of CRUD completeness
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
            try {
                _logger.LogDebug($"Received request to get beep record by client generated key {key}");
                var record = await _beepTrackerDbContext.BeepRecords.Where(b => b.ClientGeneratedKey == key).Include(b => b.BeepEntries).SingleOrDefaultAsync();
                if (record is null)
                {
                    _logger.LogDebug($"No beep record found for key of {key}");
                    return null;
                }
                var recordDto = _mapper.Map<BeepRecordDto>(record);
                _logger.LogDebug($"Returning record for key {key}, record is {recordDto}");
                return recordDto;
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting beep record by client generated key of {key}");
                throw; // bubble up
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(BeepRecordDto beepRecord)
        {
            try
            {
                _logger.LogDebug($"Received request to CREATE beep record of {beepRecord}");
                var record = _mapper.Map<BeepRecord>(beepRecord);
                await _beepTrackerDbContext.BeepRecords.AddAsync(record);
                await _beepTrackerDbContext.SaveChangesAsync();

                _logger.LogDebug($"Beep record created with id of {beepRecord.Id}");
                var result = CreatedAtAction(nameof(GetById), new { id = beepRecord.Id }, beepRecord);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while saving beep record of {beepRecord}");
                throw;
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update(BeepRecordDto beepRecord)
        {
            try
            {
                _logger.LogDebug($"Received request to UPDATE beep record of {beepRecord}");
                var record = _mapper.Map<BeepRecord>(beepRecord);
                // this does not automatically remove/update beepentry child items, so every save we end up with duplicate beepentry records
                // they get duplicated because we don't have an index with them on the device - so they come in with index = 0
                // which means they get saved as new entries
                // we'll manaully remove any existing ones
                var beepEntriesInDatabase = _beepTrackerDbContext.BeepEntries.Where(b => b.BeepRecordId == beepRecord.Id);
                foreach (var beepEntry in beepEntriesInDatabase)
                {
                    _beepTrackerDbContext.Remove(beepEntry);
                }
                _beepTrackerDbContext.BeepRecords.Update(record);
                await _beepTrackerDbContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while updating beep record of {beepRecord}");
                throw;
            }
        }

        // unsure if to be used, included for now for sake of CRUD completeness
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
