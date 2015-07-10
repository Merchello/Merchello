namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// The product tax calculation result.
    /// </summary>
    public class ProductTaxCalculationResult : IProductTaxCalculationResult
    {
        /// <summary>
        /// Gets or sets the price result.
        /// </summary>
        public ITaxCalculationResult PriceResult { get; set; }

        /// <summary>
        /// Gets or sets the sale price result.
        /// </summary>
        public ITaxCalculationResult SalePriceResult { get; set; }
    }
}