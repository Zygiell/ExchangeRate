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