using ExchangeRateAPI.Models;

namespace ExchangeRateAPI.Services
{
    public interface IExchangeRateService
    {
        Task<IEnumerable<ExchangeRateViewModel>> GetExchangeRate(KeyValuePair<string, string> currencyCodes,
            DateTime startDate, DateTime endDate);
    }
}