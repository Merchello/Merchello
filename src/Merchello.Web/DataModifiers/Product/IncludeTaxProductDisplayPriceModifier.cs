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
    public class IncludeTaxInProductPriceDataModifierTask : ProductVariantDataModifierTaskBase
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
        public override Attempt<IProductVariantDataModifierData> PerformTask(IProductVariantDataModifierData value)
        {
            var taxationContent = MerchelloContext.Gateways.Taxation;
            if (!taxationContent.ProductPricingEnabled || !value.Taxable) return Attempt<IProductVariantDataModifierData>.Succeed(value);

            var result = taxationContent.CalculateTaxesForProduct(value);
            value.AlterProduct(result);            
            return Attempt<IProductVariantDataModifierData>.Succeed(value);
        }
    }
}