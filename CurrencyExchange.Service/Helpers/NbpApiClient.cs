using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CurrencyExchange.Service.Models;

namespace CurrencyExchange.Service.Helpers
{
    public class NbpApiClient
    {
        private static readonly HttpClient http = new HttpClient();

        public async Task<List<NbpTable>> GetExchangeRatesAsync()
        {
            try
            {
                var response = await http.GetAsync("https://api.nbp.pl/api/exchangerates/tables/a/");
                response.EnsureSuccessStatusCode();
                var exchangeRates = JsonConvert.DeserializeObject<List<NbpTable>>(response.Content.ReadAsStringAsync().Result);
                return exchangeRates;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching exchange rates from NBP API", ex);
            }
        }

    }
}