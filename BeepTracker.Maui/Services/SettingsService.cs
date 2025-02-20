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
        string ApiUsername { get; set; }
        string ApiPassword { get; set; }
        string BirdListJson { get; set; }
        string OrganisationListJson { get; set; }
        bool AttemptToSyncRecords { get; set; }
        int CurrentOrganisationId { get; set; }
        List<Bird> BirdListFromDatabase { get; }
        List<Organisation> OrganisationListFromDatabase { get; }
    }

    public sealed class SettingsService : ISettingsService
    {
        private const string apiBasePath = "api_base_path";
        private const string apiBasePathDefault = "";

        private const string apiUsername = "api_username";
        private const string apiUsernameDefault = "";

        private const string apiPassword = "api_password";
        private const string apiPasswordDefault = "";

        private const string currentOrganisationId = "current_organisation_id";
        private const string currentOrganisationIdDefault = "-1";

        private const string birdListJson = "bird_list";
        private const string birdListJsonDefault = "";

        private const string organisationListJson = "organisation_list";
        private const string organisationListJsonDefault = "";

        private const string attemptToSyncRecords = "attempt_to_sync_records";
        private const bool attemptToSyncRecordsDefault = false;

        public string ApiBasePath
        {
            get => Preferences.Get(apiBasePath, apiBasePathDefault);
            set => Preferences.Set(apiBasePath, value);
        }
        public string ApiUsername
        {
            get => Preferences.Get(apiUsername, apiUsernameDefault);
            set => Preferences.Set(apiUsername, value);
        }
        public string ApiPassword
        {
            get => Preferences.Get(apiPassword, apiPasswordDefault);
            set => Preferences.Set(apiPassword, value);
        }
        public int CurrentOrganisationId
        {
            get => Preferences.Get(currentOrganisationId, int.Parse(currentOrganisationIdDefault));
            set => Preferences.Set(currentOrganisationId, value);
        }

        // a serialized list of birds that exist in our database (i.e. birds that can have beeprecords recorded)
        public string BirdListJson
        {
            get => Preferences.Get(birdListJson, birdListJsonDefault);
            set => Preferences.Set(birdListJson, value);
        }

        // deserialize the list of birds and present it as a property that can be retrieved
        public List<Bird> BirdListFromDatabase
        {
            get
            {
                if (string.IsNullOrEmpty(BirdListJson))
                {
                    return new List<Bird>();
                }
                var birds = JsonSerializer.Deserialize<List<Bird>>(BirdListJson);
                return birds ?? new List<Bird>();
            }
        }

        // a serialized list of organisations that exist in our database that the current user belows too
        public string OrganisationListJson
        {
            get => Preferences.Get(organisationListJson, organisationListJsonDefault);
            set => Preferences.Set(organisationListJson, value);
        }

        // deserialize the list of organisations and present it as a property that can be retrieved
        public List<Organisation> OrganisationListFromDatabase
        {
            get
            {
                if (string.IsNullOrEmpty(OrganisationListJson))
                {
                    return new List<Organisation>();
                }
                var orgs = JsonSerializer.Deserialize<List<Organisation>>(OrganisationListJson);
                return orgs ?? new List<Organisation>();
            }
        }

        public bool AttemptToSyncRecords
        {
            get => Preferences.Get(attemptToSyncRecords, attemptToSyncRecordsDefault);
            set => Preferences.Set(attemptToSyncRecords, value);
        }
    }
}
