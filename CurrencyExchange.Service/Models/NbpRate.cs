using Newtonsoft.Json;

namespace CurrencyExchange.Service.Models
{
    public class NbpRate
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;
        [JsonProperty("mid")]
        public double Mid { get; set; }
    }
}