using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CurrencyExchange.Service.Models;
using System.ServiceModel;

namespace CurrencyExchange.Service.Helpers
{
    public class NbpApiClient
    {
        private static readonly HttpClient http = new HttpClient();
        private static List<NbpTable> cachedExchangeRates;
        private static DateTime lastFetchTime;
        private static readonly TimeSpan cacheDuration = TimeSpan.FromHours(1);

        public async Task<List<NbpTable>> GetExchangeRatesAsync()
        {
            if (cachedExchangeRates != null && DateTime.Now - lastFetchTime < cacheDuration)
            {
                return cachedExchangeRates;
            }

            try
            {
                var response = await http.GetAsync("https://api.nbp.pl/api/exchangerates/tables/a/").ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var exchangeRates = JsonConvert.DeserializeObject<List<NbpTable>>(json);
                cachedExchangeRates = exchangeRates;
                lastFetchTime = DateTime.Now;
                return exchangeRates;
            }
            catch (Exception ex)
            {
                throw new FaultException<ExchangeServiceFault>(new ExchangeServiceFault { 
                    Message = "Failed to fetch exchange rates from NBP API", 
                    ErrorCode = "ApiError" 
                }, new FaultReason("Failed to fetch exchange rates from NBP API"));
            }
        }

    }
}