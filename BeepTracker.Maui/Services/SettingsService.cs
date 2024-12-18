using BeepTracker.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.Maui.Services
{
    public interface ISettingsService
    {
        string ApiBasePath { get; set; }
        string BirdListJson { get; set; }
        bool AttemptToSyncRecords { get; set; }
        List<Bird> BirdListFromDatabase { get; }
    }

    public sealed class SettingsService : ISettingsService
    {
        private const string apiBasePath = "api_base_path";
        private const string apiBasePathDefault = "";

        private const string birdListJson = "bird_list";
        private const string birdListJsonDefault = "";

        private const string attemptToSyncRecords = "attempt_to_sync_records";
        private const bool attemptToSyncRecordsDefault = false;

        public string ApiBasePath
        {
            get => Preferences.Get(apiBasePath, apiBasePathDefault);
            set => Preferences.Set(apiBasePath, value);
        }

        // a serialized list of birds that exist in our database (i.e. birds that can have beeprecords recorded)
        public string BirdListJson
        {
            get => Preferences.Get(birdListJson, birdListJsonDefault);
            set => Preferences.Set(birdListJson, value);
        }

        // deserialize the list of birds
        public List<Bird> BirdListFromDatabase
        {
            get
            {
                if(string.IsNullOrEmpty(BirdListJson))
                {
                    return new List<Bird>();
                }
                var birds = JsonSerializer.Deserialize<List<Bird>>(BirdListJson);
                return birds;
            }
        }

        public bool AttemptToSyncRecords
        {
            get => Preferences.Get(attemptToSyncRecords, attemptToSyncRecordsDefault);
            set => Preferences.Set(attemptToSyncRecords, value);
        }
    }
}
