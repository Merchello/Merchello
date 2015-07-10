namespace Merchello.Web
{
    using Merchello.Core;
    using Merchello.Core.Gateways.Taxation;
    using Merchello.Core.Models;
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// Extension methods for the <see cref="IMerchelloContext"/>.
    /// </summary>
    public static class MerchelloContextExtensions
    {
        /// <summary>
        /// Calculates taxes for a product.
        /// </summary>
        /// <param name="context">
        /// The <see cref="ITaxationContext"/>.
        /// </param>
        /// <param name="product">
        /// The <see cref="IProduct"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/>.
        /// </returns>
        public static IProductTaxCalculationResult CalculateTaxesForProduct(this ITaxationContext context, IProduct product)
        {
            return context.CalculateTaxesForProduct(product.ToProductDisplay());
        }

        /// <summary>
        /// Calculates taxes for a product variant.
        /// </summary>
        /// <param name="context">
        /// The <see cref="ITaxationContext"/>.
        /// </param>
        /// <param name="productVariant">
        /// The <see cref="IProductVariant"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/>.
        /// </returns>
        public static IProductTaxCalculationResult CalculateTaxesForProduct(this ITaxationContext context, IProductVariant productVariant)
        {
            return context.CalculateTaxesForProduct(productVariant.ToProductVariantDisplay());
        }
    }
}