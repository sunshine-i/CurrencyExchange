using System.Runtime.Serialization;

namespace CurrencyExchange.Service.Models
{
    [DataContract]
    public class ExchangeRequestDto
    {
        [DataMember]
        public string FromCurrency { get; set; }
        [DataMember]
        public string ToCurrency { get; set; }
        [DataMember]
        public double Amount { get; set; } = 0;
    }
}