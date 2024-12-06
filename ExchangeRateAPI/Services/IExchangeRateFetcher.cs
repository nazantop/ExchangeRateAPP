using ExchangeRateAPI.Models;

namespace ExchangeRateAPI.Services{
    public interface IExchangeRateFetcher{
        Task<ExchangeRate> FetchExchangeRateAsync();
    }
}