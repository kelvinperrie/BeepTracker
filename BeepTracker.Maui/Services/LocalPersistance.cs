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
        string recordPath;

        public LocalPersistance()
        {
            string basePath = FileSystem.Current.AppDataDirectory;
            recordPath = Path.Combine(basePath, "beeprecords");

            if (!Directory.Exists(recordPath))
            {
                Directory.CreateDirectory(recordPath);
            }
        }

        public List<BeepRecord> GetBeepRecords()
        {
            var records = new List<BeepRecord>();

            foreach (var filepath in Directory.EnumerateFiles(recordPath).OrderDescending())
            {
                var contents = File.ReadAllText(filepath);
                BeepRecord beepRecord = JsonConvert.DeserializeObject<BeepRecord>(contents);
                beepRecord.Filename = Path.GetFileName(filepath);
                records.Add(beepRecord);
            }
            return records;
        }

        public void SaveBeepRecord(BeepRecord beepRecord)
        {
            if (beepRecord == null) return;
            if (beepRecord.Filename == null)
            {
                beepRecord.Filename = DateTime.Now.ToString("o").Replace(":", "");
            }

            var filePath = Path.Combine(recordPath, beepRecord.Filename);

            var jsonData = JsonConvert.SerializeObject(beepRecord, Formatting.Indented);

            File.WriteAllText(filePath, jsonData);
        }
    }
}
