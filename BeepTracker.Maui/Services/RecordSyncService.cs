using AutoMapper;
using BeepTracker.ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.Maui.Services
{
    public class RecordSyncService
    {
        private readonly LocalPersistance _localPersistance;
        private readonly ClientService _clientService;
        private readonly IMapper _mapper;
        private readonly ISettingsService _settingsService;

        public RecordSyncService(LocalPersistance localPersistance, ClientService clientService, IMapper mapper,
            ISettingsService settingsService)
        {
            _localPersistance = localPersistance;
            _clientService = clientService;
            _mapper = mapper;
            _settingsService = settingsService;
        }

        public async void UploadRecords()
        {
            // get the unuploaded records
            // for each one
            // upload it
            // mark it locally as uploaded and save it

            var recordsToUpload = _localPersistance.GetBeepRecords().Where(br => br.Status == (int)BeepRecordStatus.Created);

            foreach (var record in recordsToUpload)
            {
                var remoteRecord = await _clientService.GetByClientGeneratedKey(record.ClientGeneratedKey);
                if (remoteRecord != null)
                {
                    var recordId = remoteRecord.Id;
                    var birdId = remoteRecord.BirdId;
                    // this record is already on the remote side, so we want to update it
                    // suspect this will overwrite the id? let's find out!
                    remoteRecord = _mapper.Map<ApiClient.Models.BeepRecord>(record);
                    remoteRecord.Id = recordId;
                    remoteRecord.BirdId = birdId;

                    await _clientService.UpdateBeepRecord(remoteRecord);

                    record.Status = (int)BeepRecordStatus.Uploaded;
                    _localPersistance.SaveBeepRecord(record);
                }
                else
                {
                    // this is a brand new record, very exciting!
                    remoteRecord = _mapper.Map<ApiClient.Models.BeepRecord>(record);
                    // todo - should we store bird id locally? or look it up? what if there is no match?
                    // if we don't have a bird id then we need to look it up
                    if(remoteRecord.BirdId == 0)
                    {
                        var bird = _settingsService.BirdListFromDatabase.FirstOrDefault(b => string.Equals(b.Name, remoteRecord.BirdName, StringComparison.CurrentCultureIgnoreCase));
                        if(bird != null)
                        {
                            remoteRecord.BirdId = bird.Id;
                        }
                    }

                    // do some validation?
                    if(remoteRecord.BirdId == 0)
                    {
                        record.SyncResponse = "Record does not have a bird id associated with it - cannot upload it. Either attach the record to a bird (by selecting from the dropdown) or ensure the bird name in the record matches a name from the bird list on the settings page.";
                    }
                    if(string.IsNullOrEmpty(remoteRecord.ClientGeneratedKey))
                    {
                        record.SyncResponse = "Record does not have a client generated key so cannot be uploaded - not sure how that happens ...";
                    }

                    await _clientService.SaveBeepRecord(remoteRecord);

                    record.Status = (int)BeepRecordStatus.Uploaded;
                    _localPersistance.SaveBeepRecord(record);
                }
            }

        }
    }
}
