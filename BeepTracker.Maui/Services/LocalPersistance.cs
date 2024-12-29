using MetroLog;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Compression;
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

        public async Task<string?> CreateCompressedBeepRecordsFile()
        {
            try
            {
                _logger.LogDebug("Attempting to generate compressed beep record archive");
                var compressedRecords = await GetCompressedBeepRecordsStream();

                var fileName = $"beeprecords-{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip";
                var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
                _logger.LogDebug($"Archive path established as {filePath}");

                MemoryStream? compressedLogs = (MemoryStream)compressedRecords;

                if (compressedLogs == null)
                {
                    return null;
                }

                _logger.LogDebug($"Attempting to write stream to arvhive file at {filePath}");
                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await compressedLogs.CopyToAsync(fileStream);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while generating compressed beep record archive");
                return null;
            }
        }

        public Task<Stream> GetCompressedBeepRecordsStream()
        {
            var ms = new MemoryStream();

            var beepRecordCounter = 0;
            using (var a = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                var beepFolder = new DirectoryInfo(recordPath);
                foreach (var file in beepFolder.GetFiles())
                {
                    beepRecordCounter++;
                    a.CreateEntryFromFile(file.FullName, file.Name);
                }
            }

            ms.Position = 0;

            _logger.LogDebug($"Creating a zip archive memory stream from {beepRecordCounter} beep records");
            return Task.FromResult<Stream>(ms);
        }
    }
}
