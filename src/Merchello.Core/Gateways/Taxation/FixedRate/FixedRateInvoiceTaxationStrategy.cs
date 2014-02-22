using System;
using System.Globalization;
using System.Linq;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Gateways.Taxation.FixedRate
{
    internal class FixedRateInvoiceTaxationStrategy : InvoiceTaxationStrategyBase
    {
        private readonly ITaxMethod _taxMethod;

        public FixedRateInvoiceTaxationStrategy(IInvoice invoice, IAddress taxAddress, ITaxMethod taxMethod)
            : base(invoice, taxAddress)
        {
            Mandate.ParameterNotNull(taxMethod, "countryTaxRate");
            
            _taxMethod = taxMethod;
        }


        /// <summary>
        /// Computes the invoice tax result
        /// </summary>
        /// <returns>The <see cref="IInvoiceTaxResult"/></returns>
        public override Attempt<IInvoiceTaxResult> CalculateTaxesForInvoice()
        {
            var extendedData = new ExtendedDataCollection();

            try
            {                
                var baseTaxRate = _taxMethod.PercentageTaxRate;

                extendedData.SetValue(Constants.ExtendedDataKeys.BaseTaxRate, baseTaxRate.ToString(CultureInfo.InvariantCulture));

                if (_taxMethod.HasProvinces)
                {
                    baseTaxRate = AdjustedRate(baseTaxRate, _taxMethod.Provinces.FirstOrDefault(x => x.Code == TaxAddress.Region), extendedData);
                }
                

                var visitor = new TaxableLineItemVisitor(baseTaxRate/100);
                Invoice.Items.Accept(visitor);



                var totalTax = visitor.TaxableLineItems.Sum(x => decimal.Parse(x.ExtendedData.GetValue(Constants.ExtendedDataKeys.LineItemTaxAmount)));


                return Attempt<IInvoiceTaxResult>.Succeed(
                    new InvoiceTaxResult(_taxMethod.Name, baseTaxRate, totalTax, extendedData)
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
        /// <param name="province">The <see cref="ITaxProvince"/> associated with the <see cref="ITaxMethod"/></param>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        private static decimal AdjustedRate(decimal baseRate, ITaxProvince province, ExtendedDataCollection extendedData)
        {
            if (province == null) return baseRate;
            extendedData.SetValue(Constants.ExtendedDataKeys.ProviceTaxRate, province.PercentRateAdjustment.ToString(CultureInfo.InvariantCulture));
            return province.PercentRateAdjustment + baseRate;
        }

    }
}