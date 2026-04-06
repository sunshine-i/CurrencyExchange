
using System.Runtime.Serialization;

namespace CurrencyExchange.Service.Models
{
    [DataContract]
    public class ExchangeRateDto
    {
        [DataMember]
        public string CurrencyCode { get; set; }
        [DataMember]
        public string CurrencyName { get; set; }
        [DataMember]
        public double Mid { get; set; }
    }
}