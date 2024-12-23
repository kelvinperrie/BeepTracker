using MetroLog;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.Maui.Services
{
    public class LocalPersistance
    {
        private readonly ILogger<LocalPersistance> _logger;

        private readonly string recordPath;

        public LocalPersistance(ILogger<LocalPersistance> logger)
        {
            _logger = logger;

            string basePath = FileSystem.Current.AppDataDirectory;
            recordPath = Path.Combine(basePath, "beeprecords");

            if (!Directory.Exists(recordPath))
            {
                Directory.CreateDirectory(recordPath);
            }

            _logger.LogInformation($"LocalPersistance constructed with recordPath of {recordPath}");
        }

        public List<BeepRecord> GetBeepRecords()
        {
            _logger.LogDebug("Starting request to get all beeprecords");
            var records = new List<BeepRecord>();

            foreach (var filepath in Directory.EnumerateFiles(recordPath).OrderDescending())
            {
                var contents = File.ReadAllText(filepath);
                BeepRecord beepRecord = JsonConvert.DeserializeObject<BeepRecord>(contents);
                beepRecord.Filename = Path.GetFileName(filepath);
                records.Add(beepRecord);
            }
            _logger.LogDebug($"Returning {records.Count} beeprecords");
            return records;
        }

        public BeepRecord? GetBeepRecordByFilename(string fileName)
        {
            try
            {
                _logger.LogDebug($"Getting beeprecord by name {fileName}");

                var filePath = Path.Combine(recordPath, fileName);

                if (!File.Exists(filePath))
                {
                    _logger.LogDebug($"No file found at path {filePath}");
                    return null;
                }

                var contents = File.ReadAllText(filePath);
                BeepRecord beepRecord = JsonConvert.DeserializeObject<BeepRecord>(contents);
                // put the filepath in the record so that we know what to save it back as
                beepRecord.Filename = Path.GetFileName(filePath);

                return beepRecord;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting beep record with filename of {fileName}");
                throw; // rethrow to bubble up
            }
        }

        public void SaveBeepRecord(BeepRecord beepRecord)
        {
            try
            {
                _logger.LogDebug($"Saving beeprecord {beepRecord}");

                if (beepRecord == null) return;
                if (beepRecord.Filename == null)
                {
                    beepRecord.Filename = DateTime.Now.ToString("o").Replace(":", "");
                    _logger.LogDebug($"Beeprecord is new; generated filename of {beepRecord.Filename}");
                }

                var filePath = Path.Combine(recordPath, beepRecord.Filename);

                var jsonData = JsonConvert.SerializeObject(beepRecord, Formatting.Indented);

                File.WriteAllText(filePath, jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while saving beep record {beepRecord}");
                throw; // rethrow to bubble up
            }
        }

        public void DeleteBeepRecord(BeepRecord beepRecord)
        {
            try
            {
                _logger.LogDebug($"Deleting beeprecord {beepRecord}");

                if (beepRecord == null) return;
                if (beepRecord.Filename == null)
                {
                    throw new Exception("Beep record has no filename, which indicates it has never been saved (so there is nothing to delete)");
                }

                var filePath = Path.Combine(recordPath, beepRecord.Filename);
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting beep record {beepRecord}");
                throw; // rethrow to bubble up
            }
        }
    }
}
