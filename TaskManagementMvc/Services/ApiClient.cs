using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace TaskManagementMvc.Services
{
    public class ApiClient : IApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiClient(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await CreateClient().GetAsync(endpoint);
            return await ReadResponse<T>(response);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request)
        {
            var response = await CreateClient().PostAsJsonAsync(endpoint, request);
            return await ReadResponse<TResponse>(response);
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest request)
        {
            var response = await CreateClient().PutAsJsonAsync(endpoint, request);
            return await ReadResponse<TResponse>(response);
        }

        public async Task DeleteAsync(string endpoint)
        {
            var response = await CreateClient().DeleteAsync(endpoint);
            await EnsureSuccess(response);
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]!);

            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        private async Task<T?> ReadResponse<T>(HttpResponseMessage response)
        {
            await EnsureSuccess(response);

            var body = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(body))
                return default;

            if (typeof(T) == typeof(string))
                return (T)(object)body;

            return JsonSerializer.Deserialize<T>(body, _jsonOptions);
        }

        private static async Task EnsureSuccess(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return;

            var message = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(message))
                message = response.ReasonPhrase ?? "API request failed.";

            throw new ApiRequestException(message, (int)response.StatusCode);
        }
    }
}
