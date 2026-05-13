using CurrencyExchange.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace CurrencyExchange.Service.Helpers
{
    public class ExchangeCalculator
    {
        public ExchangeResultDto Calculate(ExchangeRequestDto request, IEnumerable<ExchangeRateDto> rates)
        {
            if (request == null || rates == null)
            {
                throw new FaultException<ExchangeServiceFault>(new ExchangeServiceFault
                {
                    Message = "Request or rates cannot be null.",
                    ErrorCode = "InvalidInput"
                }, new FaultReason("Invalid input"));
            }

            if (request.Amount <= 0)
            {
                throw new FaultException<ExchangeServiceFault>(new ExchangeServiceFault
                {
                    Message = "Amount must be greater than zero.",
                    ErrorCode = "InvalidAmount"
                }, new FaultReason("Invalid amount"));
            }

            if (request.FromCurrency.Equals(request.ToCurrency, StringComparison.OrdinalIgnoreCase))
            {
                throw new FaultException<ExchangeServiceFault>(new ExchangeServiceFault
                {
                    Message = "FromCurrency and ToCurrency cannot be the same.",
                    ErrorCode = "InvalidCurrency"
                }, new FaultReason("Invalid currency"));
            }

            var fromRate = rates.FirstOrDefault(r => r.CurrencyCode.Equals(request.FromCurrency, StringComparison.OrdinalIgnoreCase));
            var toRate = rates.FirstOrDefault(r => r.CurrencyCode.Equals(request.ToCurrency, StringComparison.OrdinalIgnoreCase));

            var exchangeAmount = 0.0;

            if (request.FromCurrency.Equals("PLN", StringComparison.OrdinalIgnoreCase))
            {
                if (toRate == null)
                {
                    throw new FaultException<ExchangeServiceFault>(new ExchangeServiceFault
                    {
                        Message = $"Exchange rate not found for {request.FromCurrency} to {request.ToCurrency}",
                        ErrorCode = "RateNotFound"
                    }, new FaultReason("Exchange rate not found"));
                }
                exchangeAmount = request.Amount / toRate.Mid;
            }
            else if (request.ToCurrency.Equals("PLN", StringComparison.OrdinalIgnoreCase))
            {
                if (fromRate == null)
                {
                    throw new FaultException<ExchangeServiceFault>(new ExchangeServiceFault
                    {
                        Message = $"Exchange rate not found for {request.FromCurrency} to {request.ToCurrency}",
                        ErrorCode = "RateNotFound"
                    }, new FaultReason("Exchange rate not found"));
                }
                exchangeAmount = request.Amount * fromRate.Mid;
            }
            else
            {
                if (fromRate == null || toRate == null)
                {
                    throw new FaultException<ExchangeServiceFault>(new ExchangeServiceFault
                    {
                        Message = $"Exchange rate not found for {request.FromCurrency} to {request.ToCurrency}",
                        ErrorCode = "RateNotFound"
                    }, new FaultReason("Exchange rate not found"));
                }
                exchangeAmount = request.Amount * fromRate.Mid / toRate.Mid;
            }

            var result = new ExchangeResultDto
            {
                Amount = exchangeAmount,
                Rate = new List<RateInfoDto> {
                    new RateInfoDto
                    {
                        Currency = request.FromCurrency.Equals("PLN", StringComparison.OrdinalIgnoreCase) ?
                            "Polish Zloty" : fromRate?.CurrencyName,
                        Code = request.FromCurrency.Equals("PLN", StringComparison.OrdinalIgnoreCase) ?
                            "PLN" : fromRate?.CurrencyCode,
                        Mid = request.FromCurrency.Equals("PLN", StringComparison.OrdinalIgnoreCase) ?
                            1.0 : fromRate.Mid
                    },
                    new RateInfoDto
                    {
                        Currency = toRate?.CurrencyName,
                        Code = request?.ToCurrency,
                        Mid = toRate?.Mid ?? 0.0
                    }
                },
                Timestamp = DateTime.Now
            };
            return result;
        }
    }
}