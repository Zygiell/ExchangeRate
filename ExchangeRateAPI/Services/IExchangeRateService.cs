using ExchangeRateAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Services
{
    public interface IExchangeRateService
    {
        Task<IEnumerable<ExchangeRateViewModel>> GetExchangeRate(KeyValuePair<string, string> currencyCodes,
            DateTime startDate, DateTime endDate);
    }
}
