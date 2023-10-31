using System.Text.Json;
using Orders.Shared.Responses;

namespace Orders.Backend.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _client;
        private readonly string _tokenName;
        private readonly string _tokenValue;

        public ApiService(IConfiguration configuration, HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri(configuration["CoutriesBackend:urlBase"]!);
            _tokenName = configuration["CoutriesBackend:tokenName"]!;
            _tokenValue = configuration["CoutriesBackend:tokenValue"]!;
        }

        private JsonSerializerOptions _jsonDefaultOptions => new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public async Task<Response<T>> GetAsync<T>(string servicePrefix, string controller)
        {
            try
            {
                _client.DefaultRequestHeaders.Add(_tokenName, _tokenValue);
                var url = $"{servicePrefix}{controller}";
                var responseHttp = await _client.GetAsync(url);
                var response = await responseHttp.Content.ReadAsStringAsync();
                if (!responseHttp.IsSuccessStatusCode)
                {
                    return new Response<T>
                    {
                        WasSuccess = false,
                        Message = response
                    };
                }

                return new Response<T>
                {
                    WasSuccess = true,
                    Result = JsonSerializer.Deserialize<T>(response, _jsonDefaultOptions)!
                };
            }
            catch (Exception ex)
            {
                return new Response<T>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}