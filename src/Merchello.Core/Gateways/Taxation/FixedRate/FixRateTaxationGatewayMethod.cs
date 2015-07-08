namespace Merchello.Core.Gateways.Taxation.FixedRate
{
    using System.Globalization;
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models;

    using Umbraco.Core.Logging;

    /// <summary>
    /// The fix rate taxation gateway method.
    /// </summary>
    public class FixRateTaxationGatewayMethod : TaxationGatewayMethodBase, IFixedRateTaxationGatewayMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixRateTaxationGatewayMethod"/> class.
        /// </summary>
        /// <param name="taxMethod">
        /// The tax method.
        /// </param>
        public FixRateTaxationGatewayMethod(ITaxMethod taxMethod)
            : base(taxMethod)
        {
        }

        /// <summary>
        /// The calculate tax for invoice.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="taxAddress">
        /// The tax address.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/>.
        /// </returns>
        public override ITaxCalculationResult CalculateTaxForInvoice(IInvoice invoice, IAddress taxAddress)
        {            
            var ctrValues = new object[] { invoice, taxAddress, TaxMethod };

            var typeName = MerchelloConfiguration.Current.GetStrategyElement(Constants.StrategyTypeAlias.DefaultInvoiceTaxRateQuote).Type;

            var attempt = ActivatorHelper.CreateInstance<TaxCalculationStrategyBase>(typeName, ctrValues);

            if (!attempt.Success)
            {
                LogHelper.Error<FixedRateTaxationGatewayProvider>("Failed to instantiate the tax calculation strategy '" + typeName + "'", attempt.Exception);
                throw attempt.Exception;
            }

            return CalculateTaxForInvoice(attempt.Result);
        }


        /// <summary>
        /// Calculates taxes for a product.
        /// </summary>
        /// <param name="product">
        /// The <see cref="IModifiableProductVariantData"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/>.
        /// </returns>
        public virtual ITaxCalculationResult CalculateTaxForProduct(IModifiableProductVariantData product)
        {
            var extendedData = new ExtendedDataCollection();

            var baseTaxRate = TaxMethod.PercentageTaxRate;

            extendedData.SetValue(Core.Constants.ExtendedDataKeys.BaseTaxRate, baseTaxRate.ToString(CultureInfo.InvariantCulture));

            var taxRate = baseTaxRate > 1 ? baseTaxRate / 100 : baseTaxRate;

            var adjusted = product.OnSale ? product.SalePrice * taxRate : product.Price * taxRate;

            extendedData.SetValue(Constants.ExtendedDataKeys.LineItemTaxAmount, adjusted.ToString(CultureInfo.InvariantCulture));

            return new TaxCalculationResult(TaxMethod.Name, taxRate, adjusted, extendedData);
        }
    }
}