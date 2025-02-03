using BeepTracker.ApiClient.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.ApiClient
{
    public class ClientService
    {
        ILogger<ClientService> _logger;

        private readonly HttpClient _httpClient;
        private string _username;
        private string _password;

        public ClientService(ClientOptions clientOptions, ILogger<ClientService> logger)
        {
            // for this to work from a work machine it needs proxy access
            //var handler = new HttpClientHandler
            //{
            //    Proxy = new WebProxy
            //    {
            //        Address = new Uri($"http://:3128"),
            //        Credentials = new NetworkCredential("","","")
            //    },
            //};
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new System.Uri(clientOptions.BaseAddress);

            _logger = logger;
        }

        public void SetUsernameAndPassword(string username, string password)
        {
            _logger.LogInformation("Setting username and password in API client service");
            _username = username;
            _password = password;

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var byteArray = Encoding.ASCII.GetBytes(_username + ":" + _password);

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public void SetBaseAddress(string baseAddress)
        {
            _logger.LogInformation($"Setting API baseAddress in API client service to {baseAddress}");
            Uri uri = null;
            try
            {
                uri = new System.Uri(baseAddress);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while setting base address in API client service");
            }
            finally
            {
                _httpClient.BaseAddress = uri;
            }
        }

        public async Task<List<Bird>> GetBirds()
        {
            _logger.LogDebug($"Request to GetBirds recieved");
            var res = await _httpClient.GetAsync($"/api/Bird");
            if (!res.IsSuccessStatusCode)
            {
                var error = res.ReasonPhrase + " - " + res.Content.ReadAsStringAsync().Result;
                _logger.LogError("Error occurred while getting birds: " + error);
                throw new Exception(error);
            }
            else
            {
                var birds = await res.Content.ReadFromJsonAsync<List<Bird>>();
                birds = birds ?? new List<Bird>();
                _logger.LogDebug($"Returning {birds.Count} birds");
                return birds;
            }
        }

        // not currenlty needed
        //public async Task<List<BeepRecord>> GetBeepRecords()
        //{
        //    // todo error handling etc
        //    return await _httpClient.GetFromJsonAsync<List<BeepRecord>?>("/api/{_apiVersion}/BeepRecord");
        //}


        // not currenlty needed
        //public async Task<BeepRecord?> GetById(int id)
        //{
        //    // todo error handling etc
        //    return await _httpClient.GetFromJsonAsync<BeepRecord?>($"/api/{_apiVersion}/BeepRecord/{id}");
        //}

        public async Task<BeepRecord?> GetByClientGeneratedKey(string key)
        {
            _logger.LogDebug($"Request to GetByClientGeneratedKey recieved");
            var res = await _httpClient.GetAsync($"/api/BeepRecord/GetByClientGeneratedKey/{key}");
            if (res.StatusCode == HttpStatusCode.NoContent) {
                // this happens if we didn't find the item
                return null;
            }
            else if (!res.IsSuccessStatusCode)
            {
                var error = res.ReasonPhrase + " - " + res.Content.ReadAsStringAsync().Result;
                _logger.LogError("Error occurred while getting by client generated key: " + error);
                throw new Exception(error);
            }
            else 
            {
                // presumable successful and the item was found, so we should be able to deserialize it?
                var beepRecord = await res.Content.ReadFromJsonAsync<BeepRecord>();
                return beepRecord;
            }
        }

        public async Task SaveBeepRecord(BeepRecord beepRecord)
        {
            _logger.LogDebug($"Request to SaveBeepRecord recieved");
            JsonContent content = JsonContent.Create(beepRecord);

            var res = await _httpClient.PostAsync($"/api/BeepRecord", content);
            if (!res.IsSuccessStatusCode)
            {
                var error = res.ReasonPhrase + " - " + res.Content.ReadAsStringAsync().Result;
                _logger.LogError("Error occurred while saving beep record: " + error);
                throw new Exception(error);
            }
        }

        public async Task UpdateBeepRecord(BeepRecord beepRecord)
        {
            _logger.LogDebug($"Request to UpdateBeepRecord recieved");
            JsonContent content = JsonContent.Create(beepRecord);

            var res = await _httpClient.PutAsync($"/api/BeepRecord", content);
            if (!res.IsSuccessStatusCode)
            {
                var error = res.ReasonPhrase + " - " + res.Content.ReadAsStringAsync().Result;
                _logger.LogError("Error occurred while updating beep record: " + error);
                throw new Exception(error);
            }
        }

        // not currenlty needed
        //public async Task DeleteBeepRecord(int id)
        //{
        //    // todo error handling etc
        //    await _httpClient.DeleteAsync($"/api/{_apiVersion}/BeepRecord/{id}");
        //}
    }
}
