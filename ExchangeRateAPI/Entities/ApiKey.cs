using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Entities
{
    public class ApiKey
    {
        public Guid Id { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
