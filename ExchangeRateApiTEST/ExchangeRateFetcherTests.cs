using System.Threading.Tasks;
using ExchangeRateAPI.Models;
using ExchangeRateAPI.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;
using System.Net;

namespace ExchangeRateAPITests
{
    public class ExchangeRateFetcherTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly IOptions<ExternalApiSettings> _options;
        private readonly IExchangeRateFetcher _fetcher;

        public ExchangeRateFetcherTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://example.com")
            };

            _options = Options.Create(new ExternalApiSettings
            {
                ExchangeRateApi = "http://example.com/api/exchange-rates"
            });

            _fetcher = new ExchangeRateFetcher(_httpClient, _options);
        }

        [Fact]
        public async Task FetchExchangeRateAsync_ShouldReturnValidResponse_WhenApiReturnsData()
        {
            var mockResponse = new ExchangeRate
            {
                Base = "USD",
                Rates = new Dictionary<string, decimal>
                {
                    { "EUR", 0.85m }
                }
            };
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(mockResponse))
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage);

            var result = await _fetcher.FetchExchangeRateAsync();

            Assert.NotNull(result);
            Assert.Equal("USD", result.Base);
            Assert.True(result.Rates.ContainsKey("EUR"));
            Assert.Equal(0.85m, result.Rates["EUR"]);
        }

        [Fact]
        public async Task FetchExchangeRateAsync_ShouldThrowArgumentNullException_WhenApiReturnsNull()
        {
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("null")
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage);

            await Assert.ThrowsAsync<ArgumentNullException>(() => _fetcher.FetchExchangeRateAsync());
        }
    }
}