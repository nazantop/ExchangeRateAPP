using ExchangeRateAPI.Models;

namespace ExchangeRateAPI.Services
{
    public interface IExchangeRateService
    {
        Task<ExchangeRate> GetExchangeRatesAsync();
    }
}