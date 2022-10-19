using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Auth
{
    public class AuthenticationSettings
    {
        public string JwtKey { get; set; }
        public string JwtIssuer { get; set; }
    }
}
