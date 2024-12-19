using BeepTracker.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BeepTracker.ApiClient
{
    public class ClientService
    {
        private readonly HttpClient _httpClient;

        public ClientService(ClientOptions clientOptions)
        {
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
                // todo unsure how we're going to do loggin here
            }
            finally
            {
                _httpClient.BaseAddress = uri;
            }
        }

        public async Task<List<Bird>> GetBirds()
        {
            return await _httpClient.GetFromJsonAsync<List<Bird>?>("/api/Bird");
        }

        public async Task<List<BeepRecord>> GetBeepRecords()
        {
            return await _httpClient.GetFromJsonAsync<List<BeepRecord>?>("/api/BeepRecord");
        }

        public async Task<BeepRecord?> GetById(int id)
        {
            return await _httpClient.GetFromJsonAsync<BeepRecord?>($"/api/BeepRecord/{id}");
        }

        public async Task SaveBeepRecord(BeepRecord beepRecord)
        {
            await _httpClient.PostAsJsonAsync("/api/BeepRecord", beepRecord);
        }

        public async Task UpdateBeepRecord(BeepRecord beepRecord)
        {
            await _httpClient.PutAsJsonAsync("/api/BeepRecord", beepRecord);
        }
        public async Task DeleteBeepRecord(int id)
        {
            await _httpClient.DeleteAsync($"/api/BeepRecord/{id}");
        }
    }
}
