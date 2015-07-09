namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Defines the ProductTaxCalculationResult.
    /// </summary>
    public interface IProductTaxCalculationResult
    {
        /// <summary>
        /// Gets or sets the price result.
        /// </summary>
        ITaxCalculationResult PriceResult { get; set; }

        /// <summary>
        /// Gets or sets the sale price result.
        /// </summary>
        ITaxCalculationResult SalePriceResult { get; set; }
    }
}