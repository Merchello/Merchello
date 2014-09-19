namespace Merchello.Core.Gateways.Taxation.FixedRate
{
    using System;
    using System.Globalization;
    using System.Linq;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The fixed rate tax calculation strategy.
    /// </summary>
    internal class FixedRateTaxCalculationStrategy : TaxCalculationStrategyBase
    {
        /// <summary>
        /// The tax method.
        /// </summary>
        private readonly ITaxMethod _taxMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedRateTaxCalculationStrategy"/> class.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="taxAddress">
        /// The tax address.
        /// </param>
        /// <param name="taxMethod">
        /// The tax method.
        /// </param>
        public FixedRateTaxCalculationStrategy(IInvoice invoice, IAddress taxAddress, ITaxMethod taxMethod)
            : base(invoice, taxAddress)
        {
            Mandate.ParameterNotNull(taxMethod, "countryTaxRate");
            
            _taxMethod = taxMethod;
        }

        /// <summary>
        /// Computes the invoice tax result
        /// </summary>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/>
        /// </returns>
        public override Attempt<ITaxCalculationResult> CalculateTaxesForInvoice()
        {
            var extendedData = new ExtendedDataCollection();

            try
            {                
                var baseTaxRate = _taxMethod.PercentageTaxRate;

                extendedData.SetValue(Core.Constants.ExtendedDataKeys.BaseTaxRate, baseTaxRate.ToString(CultureInfo.InvariantCulture));

                if (_taxMethod.HasProvinces)
                {
                    baseTaxRate = AdjustedRate(baseTaxRate, _taxMethod.Provinces.FirstOrDefault(x => x.Code == TaxAddress.Region), extendedData);
                }
                
                var visitor = new TaxableLineItemVisitor(baseTaxRate / 100);

                Invoice.Items.Accept(visitor);

                var totalTax = visitor.TaxableLineItems.Sum(x => decimal.Parse(x.ExtendedData.GetValue(Core.Constants.ExtendedDataKeys.LineItemTaxAmount), CultureInfo.InvariantCulture));

                return Attempt<ITaxCalculationResult>.Succeed(
                    new TaxCalculationResult(_taxMethod.Name, baseTaxRate, totalTax, extendedData));
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
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>The tax adjustment</returns>
        private static decimal AdjustedRate(decimal baseRate, ITaxProvince province, ExtendedDataCollection extendedData)
        {
            if (province == null) return baseRate;
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.ProviceTaxRate, province.PercentAdjustment.ToString(CultureInfo.InvariantCulture));
            return province.PercentAdjustment + baseRate;
        }
    }
}
