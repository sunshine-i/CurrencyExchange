using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CurrencyExchange.Service.Models
{
    public class NbpTable
    {
        [JsonProperty("table")]
        public string Table { get; set; }
        [JsonProperty("effectiveDate")]
        public DateTime EffectiveDate { get; set; }
        [JsonProperty("rates")]
        public List<NbpRate> Rates { get; set; }

    }
}