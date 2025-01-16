using Microsoft.Extensions.Configuration;
using ProjectLibrary.DTO;
using ProjectLibrary.Models;
using System.Text;
using System.Text.Json;

namespace BlazorDemo.Services
{
    public class Requests
    {
        IConfiguration _configuration;
        IHttpClientFactory _clientFactory;

        public Requests(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<PagedResult<Users>?> FetchUsers(int page, int pageSize)
        {
            string? connectionString = _configuration.GetValue<string>("API:Url") ?? Environment.GetEnvironmentVariable("API_URL");
            string url = $"{connectionString}/api/users?page={page}&pageSize={pageSize}";

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            var client = new HttpClient(handler);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<PagedResult<Users>>(responseStream);
            }

            return null;
        }

        public async Task<HttpResponseMessage> AddUser(Users user)
        {
            string? connectionString = _configuration.GetValue<string>("API:Url") ?? Environment.GetEnvironmentVariable("API_URL");
            string url = $"{connectionString}/api/users/add";

            var json = JsonSerializer.Serialize(user);

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var client = _clientFactory.CreateClient();

            return await client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> PutUser(Users user)
        {
            string? connectionString = _configuration.GetValue<string>("API:Url") ?? Environment.GetEnvironmentVariable("API_URL");
            string url = $"{connectionString}/api/users/set/{user.Id}";

            var json = JsonSerializer.Serialize(user);

            var request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var client = _clientFactory.CreateClient();

            return await client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> DeleteUser(Users user)
        {
            string? connectionString = _configuration.GetValue<string>("API:Url") ?? Environment.GetEnvironmentVariable("API_URL");
            string url = $"{connectionString}/api/users/delete/{user.Id}";

            var request = new HttpRequestMessage(HttpMethod.Delete, url);

            var client = _clientFactory.CreateClient();

            return await client.SendAsync(request);
        }
    }
}
