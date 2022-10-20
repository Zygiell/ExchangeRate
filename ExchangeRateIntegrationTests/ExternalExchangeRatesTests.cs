using ExchangeRateAPI.Exceptions;
using ExchangeRateAPI.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateIntegrationTests
{
    public class ExternalExchangeRatesTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ExternalExchangeRatesTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private IExchangeRateService ExchangeRateService
        {
            get
            {
                var scope = _factory.Server.Services.CreateScope();
                var _exchangeRateService = scope.ServiceProvider.GetService<IExchangeRateService>();

                return _exchangeRateService;
            }
        }

        public static IEnumerable<object[]> GetExchangeRatesValidParams()
        {
            yield return new object[] { new KeyValuePair<string, string>("USD", "EUR"), new DateTime(2022, 1, 10), new DateTime(2022, 1, 20) };
            yield return new object[] { new KeyValuePair<string, string>("PLN", "EUR"), new DateTime(2021, 5, 10), new DateTime(2021, 5, 22) };
            yield return new object[] { new KeyValuePair<string, string>("GBP", "EUR"), new DateTime(2021, 10, 10), new DateTime(2022, 3, 4) };
        }

        [Theory]
        [MemberData(nameof(GetExchangeRatesValidParams))]
        public async Task GetExchangeRates_WithValidData_ReturnsNotEmptyCollection(
            KeyValuePair<string, string> currenciesKeyValuePair,
            DateTime startDate,
            DateTime endDate)
        {
            var result = await ExchangeRateService.GetExchangeRate(currenciesKeyValuePair, startDate, endDate);

            Assert.NotEmpty(result);
        }
        public static IEnumerable<object[]> GetExchangeRatesInvalidParams()
        {
            yield return new object[] { new KeyValuePair<string, string>("USD", "EUR"), new DateTime(2021, 1, 10), new DateTime(2005, 1, 20) };
            yield return new object[] { new KeyValuePair<string, string>("PLN", "EUR"), new DateTime(2021, 5, 10), new DateTime(1990, 5, 22) };
            yield return new object[] { new KeyValuePair<string, string>("GBP", "EUR"), new DateTime(2021, 10, 10), new DateTime(1999, 3, 4) };
            yield return new object[] { new KeyValuePair<string, string>("NonValidCurrency", "EUR"), new DateTime(2021, 10, 10), new DateTime(2042, 3, 4) };
        }

        [Theory]
        [MemberData(nameof(GetExchangeRatesInvalidParams))]
        public async Task GetExchangeRates_WithInvalidData_ThrowsException(
            KeyValuePair<string, string> currenciesKeyValuePair,
            DateTime startDate,
            DateTime endDate)
        {
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await ExchangeRateService.GetExchangeRate(currenciesKeyValuePair, startDate, endDate));
        }
    }
}
