using System;
using System.Globalization;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models;
using Umbraco.Core;
using Constants = Merchello.Core.Constants;

namespace Merchello.Web.GatewayProviders.Taxation.FixedRate
{
    internal class FixedRateTaxCalculationStrategy : TaxCalculationStrategyBase
    {
        private readonly ITaxMethod _taxMethod;

        public FixedRateTaxCalculationStrategy(IInvoice invoice, IAddress taxAddress, ITaxMethod taxMethod)
            : base(invoice, taxAddress)
        {
            Mandate.ParameterNotNull(taxMethod, "countryTaxRate");
            
            _taxMethod = taxMethod;
        }


        /// <summary>
        /// Computes the invoice tax result
        /// </summary>
        /// <returns>The <see cref="ITaxCalculationResult"/></returns>
        public override Attempt<ITaxCalculationResult> CalculateTaxesForInvoice()
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

                return Attempt<ITaxCalculationResult>.Succeed(
                    new TaxCalculationResult(_taxMethod.Name, baseTaxRate, totalTax, extendedData)
                    );
            }
            catch (Exception ex)
            {
                return Attempt<ITaxCalculationResult>.Fail(ex);
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
            extendedData.SetValue(Constants.ExtendedDataKeys.ProviceTaxRate, province.PercentAdjustment.ToString(CultureInfo.InvariantCulture));
            return province.PercentAdjustment + baseRate;
        }

    }
}