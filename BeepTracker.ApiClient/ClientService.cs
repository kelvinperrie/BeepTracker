using BeepTracker.ApiClient.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.ApiClient
{
    public class ClientService
    {
        private readonly HttpClient _httpClient;

        public ClientService(ClientOptions clientOptions)
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
        }

        public void SetBaseAddress(string baseAddress)
        {
            Uri uri = null;
            try
            {
                uri = new System.Uri(baseAddress);
            }
            catch
            {
                // todo unsure how we're going to do logging here
            }
            finally
            {
                _httpClient.BaseAddress = uri;
            }
        }

        public async Task<List<Bird>> GetBirds()
        {
            // todo error handling etc
            return await _httpClient.GetFromJsonAsync<List<Bird>?>("/api/Bird");
        }

        public async Task<List<BeepRecord>> GetBeepRecords()
        {
            // todo error handling etc
            return await _httpClient.GetFromJsonAsync<List<BeepRecord>?>("/api/BeepRecord");
        }

        public async Task<BeepRecord?> GetById(int id)
        {
            // todo error handling etc
            return await _httpClient.GetFromJsonAsync<BeepRecord?>($"/api/BeepRecord/{id}");
        }

        public async Task<BeepRecord?> GetByClientGeneratedKey(string key)
        {
            var res = await _httpClient.GetAsync($"/api/BeepRecord/GetByClientGeneratedKey/{key}");
            if (res.StatusCode == HttpStatusCode.NoContent) {
                // this happens if we didn't find the item
                return null;
            }
            else if (!res.IsSuccessStatusCode)
            {
                var error = res.Content.ReadAsStringAsync().Result;
                throw new Exception(error);
            }
            else 
            {
                // presumable successful and the item was found, so we should be able to deserialize it?
                var beepRecord = await res.Content.ReadFromJsonAsync<BeepRecord>();
                return beepRecord;
            }

            //return await _httpClient.GetFromJsonAsync<BeepRecord?>($"/api/BeepRecord/GetByClientGeneratedKey/{key}");
        }

        public async Task SaveBeepRecord(BeepRecord beepRecord)
        {
            JsonContent content = JsonContent.Create(beepRecord);

            var res = await _httpClient.PostAsync($"/api/BeepRecord", content);
            if (!res.IsSuccessStatusCode)
            {
                var error = res.Content.ReadAsStringAsync().Result;
                throw new Exception(error);
            }
        }

        public async Task UpdateBeepRecord(BeepRecord beepRecord)
        {
            JsonContent content = JsonContent.Create(beepRecord);

            var res = await _httpClient.PutAsync($"/api/BeepRecord", content);
            if (!res.IsSuccessStatusCode)
            {
                var error = res.Content.ReadAsStringAsync().Result;
                throw new Exception(error);
            }
        }

        public async Task DeleteBeepRecord(int id)
        {
            // todo error handling etc
            await _httpClient.DeleteAsync($"/api/BeepRecord/{id}");
        }
    }
}
