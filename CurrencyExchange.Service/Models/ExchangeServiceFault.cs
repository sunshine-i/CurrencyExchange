using System.Runtime.Serialization;

namespace CurrencyExchange.Service.Models
{
    [DataContract]
    public class ExchangeServiceFault
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string ErrorCode { get; set; }
    }
}