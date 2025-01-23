using AutoMapper;
using BeepTracker.Blazor.Models;
using BeepTracker.Common.Dtos;
using Microsoft.EntityFrameworkCore;
using static MudBlazor.CategoryTypes;


namespace BeepTracker.Blazor.Business
{

    public interface IBeepRecordService
    {
        IEnumerable<BeepRecordDto> GetAllForBird(int birdId);
        void Update(BeepRecordDto organisation);
    }

    public class BeepRecordService : IBeepRecordService
    {
        private readonly IMapper _mapper;
        public readonly BeepTrackerDbContext _context;

        public BeepRecordService(BeepTrackerDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<BeepRecordDto> GetAllForBird(int birdId)
        {
            return _context.BeepRecords.Include(b => b.BeepEntries.OrderBy(e => e.Id)).Where(b => b.BirdId == birdId).Select(b => _mapper.Map<BeepRecordDto>(b));
        }

        public void Update(BeepRecordDto beepRecord)
        {
            var recordToUpdate = _context.BeepRecords.FirstOrDefault(b => b.Id == beepRecord.Id);
            if (recordToUpdate != null)
            {
                // we have to manually remove the beepentry children otherwise we get relationship problems via entityframework
                // the mapper puts them all back, and somehow they maintain their original ids ... ?
                foreach (var beepEntry in recordToUpdate.BeepEntries)
                {
                    _context.Remove(beepEntry);
                }
                _mapper.Map(beepRecord, recordToUpdate);

            }
            else
            {
                var updatedRecord = _mapper.Map<BeepRecord>(beepRecord);
                _context.Add(updatedRecord);
            }
            _context.SaveChanges();
        }

    }
}
