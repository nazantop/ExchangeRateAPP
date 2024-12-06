using Microsoft.AspNetCore.Mvc;
using ExchangeRateAPI.Services;

namespace ExchangeRateAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRateController : ControllerBase
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ILogger<ExchangeRateController> _logger;

        public ExchangeRateController(IExchangeRateService exchangeRateService, ILogger<ExchangeRateController> logger)
        {
            _exchangeRateService = exchangeRateService;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetExchangeRates")]
        public async Task<IActionResult> GetExchangeRates()
        {
            try
            {
                _logger.LogInformation("Handling GetExchangeRates request.");
                var rates = await _exchangeRateService.GetExchangeRatesAsync();
                return Ok(rates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching exchange rates");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}