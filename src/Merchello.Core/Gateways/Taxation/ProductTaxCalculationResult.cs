namespace Merchello.Core.Gateways.Taxation
{
    using System.Globalization;

    using Merchello.Core.Models;

    /// <summary>
    /// The product tax calculation result.
    /// </summary>
    public class ProductTaxCalculationResult : IProductTaxCalculationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductTaxCalculationResult"/> class.
        /// </summary>
        /// <param name="taxMethodName">
        /// The tax method name
        /// </param>
        /// <param name="originalPrice">
        /// The original price.
        /// </param>
        /// <param name="modifiedPrice">
        /// The modified price.
        /// </param>
        /// <param name="originalSalePrice">
        /// The original sale price.
        /// </param>
        /// <param name="modifiedSalePrice">
        /// The modified sale price.
        /// </param>
        /// <param name="baseTaxRate">
        /// The base tax rate
        /// </param>
        public ProductTaxCalculationResult(
            string taxMethodName,
            decimal originalPrice,
            decimal modifiedPrice,
            decimal originalSalePrice,
            decimal modifiedSalePrice,
            decimal? baseTaxRate = null)
        {
            var edprice = new ExtendedDataCollection();
            var edsaleprice = new ExtendedDataCollection();

             var taxRate = baseTaxRate != null ? 
                 baseTaxRate > 1  ? 
                    baseTaxRate.Value / 100M : baseTaxRate.Value :
                    0M;

            if (baseTaxRate != null)
            {
                edprice.SetValue(Core.Constants.ExtendedDataKeys.BaseTaxRate, baseTaxRate.Value.ToString(CultureInfo.InvariantCulture));
                edsaleprice.SetValue(Core.Constants.ExtendedDataKeys.BaseTaxRate, baseTaxRate.Value.ToString(CultureInfo.InvariantCulture));
            }

            edprice.SetValue(Constants.ExtendedDataKeys.ProductPriceNoTax, originalPrice.ToString(CultureInfo.InvariantCulture));
            edprice.SetValue(Constants.ExtendedDataKeys.ProductPriceTaxAmount, modifiedPrice.ToString(CultureInfo.InvariantCulture));
            edsaleprice.SetValue(Constants.ExtendedDataKeys.ProductSalePriceNoTax, originalSalePrice.ToString(CultureInfo.InvariantCulture));
            edsaleprice.SetValue(Constants.ExtendedDataKeys.ProductSalePriceTaxAmount, modifiedSalePrice.ToString(CultureInfo.InvariantCulture));

            PriceResult = new TaxCalculationResult(taxMethodName, taxRate, modifiedPrice, edprice);
            SalePriceResult = new TaxCalculationResult(taxMethodName, taxRate, modifiedSalePrice, edsaleprice);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ProductTaxCalculationResult"/> class from being created.
        /// </summary>
        private ProductTaxCalculationResult()
        {            
        }

        /// <summary>
        /// Gets the price result.
        /// </summary>
        public ITaxCalculationResult PriceResult { get; private set; }

        /// <summary>
        /// Gets the sale price result.
        /// </summary>
        public ITaxCalculationResult SalePriceResult { get; private set; }

        /// <summary>
        /// Gets an empty ProductTaxCalculationResult.
        /// </summary>
        /// <returns>
        /// The <see cref="IProductTaxCalculationResult"/>.
        /// </returns>
        public static IProductTaxCalculationResult GetEmptyResult()
        {
            return new ProductTaxCalculationResult()
            {
                PriceResult = new TaxCalculationResult(0, 0),
                SalePriceResult = new TaxCalculationResult(0, 0)
            };
        }
    }
}