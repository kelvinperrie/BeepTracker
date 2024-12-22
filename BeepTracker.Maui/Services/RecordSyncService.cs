using AutoMapper;
using BeepTracker.ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.Maui.Services
{
    public class UploadRecordsResponse
    {
        public int totalRecordsAttempted { get; set; }
        public int uploadFailureCount { get; set; }
    }

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

        public async Task<UploadRecordsResponse> UploadRecords()
        {
            // get the unuploaded records
            // for each one
            // upload it
            // mark it locally as uploaded and save it
            

            var recordsToUpload = _localPersistance.GetBeepRecords().Where(br => br.UploadStatus == (int)BeepRecordUploadStatus.Created || br.UploadStatus == (int)BeepRecordUploadStatus.Updated);

            var recordsToUploadCount = recordsToUpload.Count();
            var failureCount = 0;


            foreach (var record in recordsToUpload)
            {
                try
                {
                    var remoteRecord = await _clientService.GetByClientGeneratedKey(record.ClientGeneratedKey);

                    //throw new Exception("test failure! something bad happened here. I don't know what but it stopped the upload.");

                    if (remoteRecord != null)
                    {
                        var recordId = remoteRecord.Id;
                        // if the local record has a bird id then use that, otherwise use the remote one
                        // is it possible that the remote one can have a bird id and the local can't??????
                        var birdId = record.birdId != 0 ? record.birdId : remoteRecord.BirdId;
                        // this record is already on the remote side, so we want to update it
                        // suspect this will overwrite the id? let's find out!
                        remoteRecord = _mapper.Map<ApiClient.Models.BeepRecord>(record);
                        remoteRecord.Id = recordId;
                        remoteRecord.BirdId = birdId;

                        await _clientService.UpdateBeepRecord(remoteRecord);

                        record.UploadStatus = (int)BeepRecordUploadStatus.Uploaded;
                        _localPersistance.SaveBeepRecord(record);
                    }
                    else
                    {
                        // this is a brand new record, very exciting!
                        remoteRecord = _mapper.Map<ApiClient.Models.BeepRecord>(record);
                        if(string.IsNullOrEmpty( remoteRecord.BirdName))
                        {
                            throw new Exception($"Record from {remoteRecord.RecordedDate} does not have a bird name. To be uploaded it needs to be associated with a bird from the dropdown.");
                        }
                        // todo - should we store bird id locally? or look it up? what if there is no match?
                        // if we don't have a bird id then we need to look it up
                        if (remoteRecord.BirdId == 0)
                        {
                            var bird = _settingsService.BirdListFromDatabase.FirstOrDefault(b => string.Equals(b.Name, remoteRecord.BirdName, StringComparison.CurrentCultureIgnoreCase));
                            if (bird != null)
                            {
                                remoteRecord.BirdId = bird.Id;
                            }
                        }

                        // do some validation?
                        if (remoteRecord.BirdId == 0)
                        {
                            throw new Exception($"Record for {remoteRecord.BirdName} does not have a bird id associated with it - cannot upload it. Either attach the record to a bird (by selecting from the dropdown) or ensure the bird name in the record matches a name from the bird list on the settings page.");
                        }
                        if (string.IsNullOrEmpty(remoteRecord.ClientGeneratedKey))
                        {
                            throw new Exception($"Record for {remoteRecord.BirdName}  does not have a client generated key so cannot be uploaded - not sure how that happens ...");
                        }

                        await _clientService.SaveBeepRecord(remoteRecord);

                        record.UploadStatus = (int)BeepRecordUploadStatus.Uploaded;
                        _localPersistance.SaveBeepRecord(record);
                    }
                } catch (Exception ex)
                {
                    var responseMessage = ex.Message;
                    record.SyncResponse = DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ": " + responseMessage;
                    record.UploadStatus = (int)BeepRecordUploadStatus.Errored;
                    _localPersistance.SaveBeepRecord(record);
                    failureCount++;
                }
            }

            var result = new UploadRecordsResponse
            {
                totalRecordsAttempted = recordsToUploadCount,
                uploadFailureCount = failureCount
            };

            return result;
        }
    }
}
