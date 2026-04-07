using System.Runtime.Serialization;

namespace CurrencyExchange.Service.Models
{
    [DataContract]
    public class RateInfoDto
    {
        [DataMember]
        public string Currency { get; set; } = string.Empty;

        [DataMember]
        public string Code { get; set; } = string.Empty;

        [DataMember]
        public double Mid { get; set; }
    }
}