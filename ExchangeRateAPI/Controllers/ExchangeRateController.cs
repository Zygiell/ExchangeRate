using ExchangeRateAPI.Models;
using ExchangeRateAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ExchangeRateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRateController : Controller
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IApiKeyService _apiKeyService;

        public ExchangeRateController(IExchangeRateService exchangeRateService, IApiKeyService apiKeyService)
        {
            _exchangeRateService = exchangeRateService;
            _apiKeyService = apiKeyService;
        }

        [HttpPost("generate")]
        [SwaggerOperation(Summary = "Generate ApiKey")]
        public async Task<string> GenerateApiKey()
        {
            return await _apiKeyService.GenerateApiKeyAsync();
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Compares and returns the exchange rates of the two given currencies: Key - Value from the given time range startDate - endDate (yyyy-MM-dd)")]
        [Authorize]
        public async Task<IEnumerable<ExchangeRateViewModel>> GetExchangeRates([FromQuery] KeyValuePair<string, string> currencyCodes,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            //KeyValuePair<string, string> currencyCodes = new KeyValuePair<string, string>(dto.FirstCurrency, dto.SecCurrency);

            return await _exchangeRateService.GetExchangeRate(currencyCodes, startDate, endDate);
        }
    }
}