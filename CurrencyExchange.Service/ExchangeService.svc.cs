using CurrencyExchange.Service.Helpers;
using CurrencyExchange.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ExchangeService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ExchangeService.svc or ExchangeService.svc.cs at the Solution Explorer and start debugging.
    public class ExchangeService : IExchangeService
    {

        private readonly NbpApiClient _nbpApiClient = new NbpApiClient();

        public async Task<List<ExchangeRateDto>> GetExchangeRates()
        {
            var exchangeRates = await _nbpApiClient.GetExchangeRatesAsync();

            return exchangeRates.FirstOrDefault()?.Rates.Select(r => new ExchangeRateDto
            {
                CurrencyCode = r.Code,
                CurrencyName = r.Currency,
                Mid = r.Mid
            }).ToList();
        }

        public async Task<double> GetExchangeRate(string currencyCode)
        {
            var exchangeRates = await _nbpApiClient.GetExchangeRatesAsync();
            return exchangeRates.FirstOrDefault()?.Rates.FirstOrDefault(r => 
                r.Code.Equals(currencyCode, StringComparison.OrdinalIgnoreCase))?.Mid ?? 0.0;
        }
    }
}