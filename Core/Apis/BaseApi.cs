using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Apis
{
    public class BaseApi
    {
        protected readonly Uri _baseAddress;
        private readonly HttpClient _restClient;

        public BaseApi()
        {
            _baseAddress = new Uri("http://localhost:5000");
            _restClient = new HttpClient
            {
                BaseAddress = _baseAddress,
                Timeout = new TimeSpan(hours: 0, minutes: 0, seconds: 10)
            };
        }

        protected async Task<(bool status, string content)> GetAsync(string resource, CancellationToken token)
        {
            HttpResponseMessage response = await _restClient.GetAsync(resource, token);
            string content = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, content);
        }

        protected async Task<(bool status, string content)> PostAsync(string resource, string jsonBody, CancellationToken token)
        {
            HttpResponseMessage response = await _restClient.PostAsync(resource, new StringContent(jsonBody), token);
            string content = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, content);
        }

        protected async Task<(bool status, string content)> DeleteAsync(string resource, CancellationToken token)
        {
            HttpResponseMessage response = await _restClient.DeleteAsync(resource, token);
            string content = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, content);
        }

        protected async Task<(bool status, string content)> PutAsync(string resource, string jsonBody, CancellationToken token)
        {
            HttpResponseMessage response = await _restClient.PutAsync(resource, new StringContent(jsonBody), token);
            string content = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, content);
        }
    }
}
