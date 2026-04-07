using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CurrencyExchange.Service.Models
{
    [DataContract]
    public class ExchangeResultDto
    {
        [DataMember]
        public double Amount { get; set; }
        [DataMember]
        public List<RateInfoDto> Rate { get; set; }
        [DataMember]
        public DateTime Timestamp { get; set; }
    }
}