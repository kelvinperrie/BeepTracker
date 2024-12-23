using AutoMapper;
using BeepTracker.ApiClient;
using MetroLog;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RecordSyncService> _logger;

        public RecordSyncService(LocalPersistance localPersistance, ClientService clientService, IMapper mapper,
            ISettingsService settingsService, ILogger<RecordSyncService> logger)
        {
            _localPersistance = localPersistance;
            _clientService = clientService;
            _mapper = mapper;
            _settingsService = settingsService;
            _logger = logger;
        }

        public async Task<UploadRecordsResponse> UploadRecords()
        {
            // get the unuploaded records
            // for each one
            // upload it
            // mark it locally as uploaded and save it

            _logger.LogInformation("Requesting records that need uploading from local persistance");
            var recordsToUpload = _localPersistance.GetBeepRecords().Where(br => br.UploadStatus == (int)BeepRecordUploadStatus.Created || br.UploadStatus == (int)BeepRecordUploadStatus.Updated);

            var recordsToUploadCount = recordsToUpload.Count();
            var failureCount = 0;


            foreach (var record in recordsToUpload)
            {
                try
                {
                    _logger.LogInformation($"Processing record for upload; record is {record.ToString()}");
                    var remoteRecord = await _clientService.GetByClientGeneratedKey(record.ClientGeneratedKey);

                    //throw new Exception("test failure! something bad happened here. I don't know what but it stopped the upload.");
                    
                    if (remoteRecord != null)
                    {
                        _logger.LogInformation($"Located an existing remote record with client key of {record.ClientGeneratedKey}");
                        var recordId = remoteRecord.Id;
                        // if the local record has a bird id then use that, otherwise use the remote one
                        // is it possible that the remote one can have a bird id and the local can't??????
                        var birdId = record.birdId != 0 ? record.birdId : remoteRecord.BirdId;
                        // this record is already on the remote side, so we want to update it
                        // suspect this will overwrite the id? let's find out!
                        remoteRecord = _mapper.Map<ApiClient.Models.BeepRecord>(record);
                        remoteRecord.Id = recordId;
                        remoteRecord.BirdId = birdId;

                        _logger.LogInformation($"Asking API to UPDATE beep record: {remoteRecord.ToString()}");
                        await _clientService.UpdateBeepRecord(remoteRecord);

                    }
                    else
                    {
                        // this is a brand new record, very exciting!
                        _logger.LogInformation($"No remote record found with client key of {record.ClientGeneratedKey}");
                        remoteRecord = _mapper.Map<ApiClient.Models.BeepRecord>(record);
                        if(string.IsNullOrEmpty( remoteRecord.BirdName))
                        {
                            throw new Exception($"Record from {remoteRecord.RecordedDate} does not have a bird name. To be uploaded it needs to be associated with a bird from the dropdown.");
                        }
                        // todo - should we store bird id locally? or look it up? what if there is no match?
                        // if we don't have a bird id then we need to look it up
                        if (remoteRecord.BirdId == 0)
                        {
                            _logger.LogInformation($"Record does not have a bird id, attempting to look up locally from bird list.");
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

                        _logger.LogInformation($"Asking API to SAVE beep record: {remoteRecord.ToString()}");
                        await _clientService.SaveBeepRecord(remoteRecord);

                    }
                    _logger.LogInformation($"Setting UploadedStatus to 'Uploaded' in local persistance");
                    record.UploadStatus = (int)BeepRecordUploadStatus.Uploaded;
                    _localPersistance.SaveBeepRecord(record);

                } catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error during record sync/upload");
                    var responseMessage = ex.Message;
                    record.SyncResponse = DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ": " + responseMessage;
                    record.UploadStatus = (int)BeepRecordUploadStatus.Errored;
                    _localPersistance.SaveBeepRecord(record);
                    failureCount++;
                } finally
                {
                    _logger.LogInformation($"Completed upload/sync process for beeprecord.");
                }
            }

            var result = new UploadRecordsResponse
            {
                totalRecordsAttempted = recordsToUploadCount,
                uploadFailureCount = failureCount
            };
            _logger.LogInformation($"Completed entire upload/sync process; totalRecordsAttempted: {result.totalRecordsAttempted}, uploadFailureCount: {result.uploadFailureCount}");

            return result;
        }
    }
}
