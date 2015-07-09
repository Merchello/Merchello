namespace Merchello.Web.DataModifiers.Product
{
    using System;
    using System.Runtime.InteropServices;

    using Merchello.Core;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The include tax in product price data modifier task.
    /// </summary>
    public class IncludeTaxInProductPriceDataModifierTask : ModifiableProductVariantDataModifierTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncludeTaxInProductPriceDataModifierTask"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public IncludeTaxInProductPriceDataModifierTask(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
        }

        /// <summary>
        /// Attempts to modify the product pricing with tax.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<IModifiableProductVariantData> PerformTask(IModifiableProductVariantData value)
        {
            var taxationContent = MerchelloContext.Gateways.Taxation;
            if (!taxationContent.ProductPricingEnabled) return Attempt<IModifiableProductVariantData>.Succeed(value);

            var result = taxationContent.CalculateTaxesForProduct(value);
            value.Price += result.PriceResult.TaxAmount;
            value.SalePrice += result.SalePriceResult.TaxAmount;

            return Attempt<IModifiableProductVariantData>.Succeed(value);
        }
    }
}