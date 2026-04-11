using System;
using System.Collections.Generic;

namespace CurrencyExchange.Client.Models
{
    public class ExchangeResult
    {
        public double Amount { get; set; }
        public List<RateInfo> Rate { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
