namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Defines the ProductTaxCalculationResult.
    /// </summary>
    public interface IProductTaxCalculationResult
    {
        /// <summary>
        /// Gets the price result.
        /// </summary>
        ITaxCalculationResult PriceResult { get; }

        /// <summary>
        /// Gets the sale price result.
        /// </summary>
        ITaxCalculationResult SalePriceResult { get; }
    }
}