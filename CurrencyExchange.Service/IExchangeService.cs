using CurrencyExchange.Service.Models;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace CurrencyExchange.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IExchangeService" in both code and config file together.
    [ServiceContract]
    public interface IExchangeService
    {
        [OperationContract]
        [FaultContract(typeof(ExchangeServiceFault))]
        Task<List<ExchangeRateDto>> GetExchangeRates();

        [OperationContract]
        [FaultContract(typeof(ExchangeServiceFault))]
        Task<double> GetExchangeRate(string currencyCode);

        [OperationContract]
        [FaultContract(typeof(ExchangeServiceFault))]
        Task<ExchangeResultDto> ExchangeCurrency(ExchangeRequestDto request);

        

    }
}
