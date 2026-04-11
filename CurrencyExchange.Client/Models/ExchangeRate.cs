namespace CurrencyExchange.Client.Models
{
    public class ExchangeRate
    {
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public double Mid { get; set; }
    }
}
