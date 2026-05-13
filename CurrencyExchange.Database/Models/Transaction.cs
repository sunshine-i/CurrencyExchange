using System;

namespace CurrencyExchange.Database.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public string Type { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public double? FromAmount { get; set; }
        public double? ToAmount { get; set; }
        public string Rate { get; set; }
        public DateTime Timestamp { get; set; }

        // Display helper shown in the history table
        public string Description
        {
            get
            {
                if (Type == "TOPUP")
                    return $"Top-up {ToAmount:N2} {ToCurrency}";

                return $"{FromAmount:N2} {FromCurrency}  →  {ToAmount:N2} {ToCurrency}";
            }
        }
    }
}
