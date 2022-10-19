using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Models
{
    public class ExchangeRateViewModel
    {
        public string FirstCurrency { get; set; }
        public string SecondCurrency { get; set; }
        public DateTime Date { get; set; }
        public decimal ExchangeRateValue { get; set; }
    }
}
