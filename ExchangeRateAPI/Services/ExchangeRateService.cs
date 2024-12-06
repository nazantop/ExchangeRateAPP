using ExchangeRateAPI.Models;
using ExchangeRateAPI.Repositories;

namespace ExchangeRateAPI.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IRedisRepository _redisRepository;
        private readonly IExchangeRateFetcher _exchangeRateFetcher;
        private readonly ILogger _logger;
        private const string CacheExchangeRateKey = "ExchangeRates";

        public ExchangeRateService(IRedisRepository redisRepository, IExchangeRateFetcher exchangeRateFetcher , ILogger<ExchangeRateService> logger)
        {
            _redisRepository = redisRepository;
            _exchangeRateFetcher = exchangeRateFetcher;
            _logger = logger;
        }

        public async Task<ExchangeRate> GetExchangeRatesAsync()
        {
           try
           {
            _logger.LogInformation("Fetching exchange rates from cache.");
            var cachedRates = await _redisRepository.GetAsync<ExchangeRate>(CacheExchangeRateKey);
            if (cachedRates != null)
            {
                _logger.LogInformation("Cache hit.");
                return cachedRates;
            }
            _logger.LogInformation("Cache miss. Fetching from API.");
            var rates = await _exchangeRateFetcher.FetchExchangeRateAsync();
            await _redisRepository.SaveAsync(CacheExchangeRateKey, rates, TimeSpan.FromSeconds(20));
            _logger.LogInformation("Saved exchange rates to cache.");

            return rates;
           }
           catch (Exception ex)
           {
            _logger.LogError(ex, "Error fetching exchange rates.");
            throw;
           }
        }
    }
}