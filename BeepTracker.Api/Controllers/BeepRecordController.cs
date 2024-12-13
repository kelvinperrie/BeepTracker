﻿using AutoMapper;
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
