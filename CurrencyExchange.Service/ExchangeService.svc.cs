using CurrencyExchange.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CurrencyExchange.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ExchangeService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ExchangeService.svc or ExchangeService.svc.cs at the Solution Explorer and start debugging.
    public class ExchangeService : IExchangeService
    {
        public List<ExchangeRateDto> GetExchangeRates()
        {
            var rates = new List<ExchangeRateDto>()
            {
                new ExchangeRateDto()
                {
                    CurrencyCode = "USD",
                    CurrencyName = "US Dollar",
                    Mid = 3.75
                }
            };
            return rates;
        }

        public double GetExchangeRate(string currencyCode)
        {
            return 0.0;
        }
    }
}