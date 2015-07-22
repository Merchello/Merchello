namespace Merchello.Core.Gateways.Taxation
{
    using Merchello.Core.Models;

    /// <summary>
    /// Defines a <see cref="ITaxationGatewayMethod"/> that can be used for product based taxation.
    /// </summary>
    public interface ITaxationByProductMethod : ITaxationGatewayMethod
    {
        /// <summary>
        /// Calculates taxes for a product.
        /// </summary>
        /// <param name="product">
        /// The <see cref="IProductVariantDataModifierData"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/>.
        /// </returns>
        IProductTaxCalculationResult CalculateTaxForProduct(IProductVariantDataModifierData product);
    }
}