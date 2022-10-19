using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Entities
{
    public class Cache
    {
        public string FirstCurrency { get; set; }
        public string SecondCurrency { get; set; }
        public DateTime Date { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal ExchangeRateValue { get; set; }
    }
}
