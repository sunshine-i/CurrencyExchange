using CurrencyExchange.Service.Models;
using System.Collections.Generic;
using System.ServiceModel;

namespace CurrencyExchange.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IExchangeService" in both code and config file together.
    [ServiceContract]
    public interface IExchangeService
    {
        [OperationContract]
        List<ExchangeRateDto> GetExchangeRates();

        [OperationContract]
        double GetExchangeRate(string currencyCode);
    }
}
