using ExchangeRateAPI.Models;
using Microsoft.Extensions.Options;

namespace ExchangeRateAPI.Services
{
    public class ExchangeRateFetcher : IExchangeRateFetcher {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public ExchangeRateFetcher(HttpClient httpClient, IOptions<ExternalApiSettings> options) {
            _httpClient = httpClient;
            _apiUrl = options.Value.ExchangeRateApi;
        }
        public async Task<ExchangeRate> FetchExchangeRateAsync() {

            try
            {
                var response = await _httpClient.GetFromJsonAsync<ExchangeRate>(_apiUrl);
                if (response != null) 
                {
                    return response;
                }
                throw new ArgumentNullException();
            }
            catch (Exception)
            {
                
                throw;
            }           
        }
    }
}