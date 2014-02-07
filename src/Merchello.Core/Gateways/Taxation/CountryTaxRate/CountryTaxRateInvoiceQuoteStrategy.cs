using System;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Gateways.Taxation.CountryTaxRate
{
    internal class CountryTaxRateInvoiceQuoteStrategy : InvoiceTaxationStrategyBase
    {
        private readonly ICountryTaxRate _countryTaxRate;

        public CountryTaxRateInvoiceQuoteStrategy(IInvoice invoice, IAddress taxAddress, ICountryTaxRate countryTaxRate)
            : base(invoice, taxAddress)
        {
            Mandate.ParameterNotNull(countryTaxRate, "countryTaxRate");

            _countryTaxRate = countryTaxRate;
        }

        /// <summary>
        /// Computes the invoice tax result
        /// </summary>
        /// <returns>The <see cref="IInvoiceTaxResult"/></returns>
        public override Attempt<IInvoiceTaxResult> GetInvoiceTaxResult()
        {
            throw new NotImplementedException();
        }
    }
}