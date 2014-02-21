using System;
using System.Linq;
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
            try
            {                
                var baseTaxRate = _countryTaxRate.PercentageTaxRate;
                if (_countryTaxRate.HasProvinces)
                {
                    baseTaxRate = AdjustedRate(baseTaxRate, _countryTaxRate.Provinces.FirstOrDefault(x => x.Code == TaxAddress.Region));
                }
                

                var visitor = new TaxableLineItemVisitor();
                Invoice.Items.Accept(visitor);

                var taxablePrice = visitor.TaxableLineItems.Sum(x => x.Price);


                return Attempt<IInvoiceTaxResult>.Succeed(
                    new InvoiceTaxResult(baseTaxRate, taxablePrice * (baseTaxRate / 100))
                    );
            }
            catch (Exception ex)
            {
                return Attempt<IInvoiceTaxResult>.Fail(ex);
            }
                                   
        }



        /// <summary>
        /// Adjusts the rate of the quote based on the province 
        /// </summary>
        /// <param name="baseRate">The base (unadjusted) rate</param>
        /// <param name="province">The <see cref="ITaxProvince"/> associated with the <see cref="ICountryTaxRate"/></param>
        /// <returns></returns>
        private decimal AdjustedRate(decimal baseRate, ITaxProvince province)
        {
            if (province == null) return baseRate;
            return province.PercentRateAdjustment + baseRate;
        }

    }
}