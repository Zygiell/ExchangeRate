using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

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