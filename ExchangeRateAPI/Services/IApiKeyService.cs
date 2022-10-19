using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Services
{
    public interface IApiKeyService
    {
        Task<string> GenerateApiKeyAsync();
    }
}
