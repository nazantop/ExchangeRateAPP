using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using ExchangeRateAPI.Models;
using ExchangeRateAPI.Services;
using ExchangeRateAPI.Repositories;

public class ExchangeRateServiceTests
{
    private readonly Mock<IExchangeRateFetcher> _mockFetcher;
    private readonly Mock<ILogger<ExchangeRateService>> _mockLogger;
    private readonly ExchangeRateService _service;
     private readonly Mock<IRedisRepository> _repository;


    public ExchangeRateServiceTests()
    {
        _repository = new Mock<IRedisRepository>();
        _mockFetcher = new Mock<IExchangeRateFetcher>();
        _mockLogger = new Mock<ILogger<ExchangeRateService>>();
        _service = new ExchangeRateService(_repository.Object, _mockFetcher.Object, _mockLogger.Object);
    }

  [Fact]
    public async Task FetchExchangeRate_ShouldReturnRates_WhenFetcherSucceeds()
    {
        var mockExchangeRate = new ExchangeRate
        {
            Base = "USD",
            Rates = new System.Collections.Generic.Dictionary<string, decimal>
            {
                { "EUR", 0.85M },
                { "GBP", 0.75M }
            }
        };

        _mockFetcher
            .Setup(f => f.FetchExchangeRateAsync())
            .ReturnsAsync(mockExchangeRate);

        var result = await _service.GetExchangeRatesAsync();

        Assert.NotNull(result);
        Assert.Equal("USD", result.Base);
        Assert.Equal(0.85M, result.Rates["EUR"]);
        Assert.Equal(0.75M, result.Rates["GBP"]);

        _mockFetcher.Verify(f => f.FetchExchangeRateAsync(), Times.Once);
    }
   

    [Fact]
    public async Task FetchExchangeRate_ShouldThrowException_WhenFetcherFails()
    {
        _mockFetcher
            .Setup(f => f.FetchExchangeRateAsync())
            .ThrowsAsync(new Exception("Fetcher failed"));

        await Assert.ThrowsAsync<Exception>(() => _service.GetExchangeRatesAsync());

        _mockFetcher.Verify(f => f.FetchExchangeRateAsync(), Times.Once);
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Once
        );
    }
}