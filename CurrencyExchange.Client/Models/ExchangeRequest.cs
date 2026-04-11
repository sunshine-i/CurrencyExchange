namespace CurrencyExchange.Client.Models
{
    public class ExchangeRequest
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public double Amount { get; set; }
    }
}
