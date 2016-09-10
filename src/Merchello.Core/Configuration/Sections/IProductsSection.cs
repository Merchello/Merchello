namespace Merchello.Core.Configuration.Sections
{
    /// <summary>
    /// Represents a configuration section for configurations related to Merchello "products". 
    /// </summary>
    public interface IProductsSection : IMerchelloSection
    {
        /// <summary>
        /// Gets the default SKU separator used when generating product variants based off product options.
        /// <para>
        /// Example: If DefaultSkuSeparator = "-" and given a product with SKU = "p" and option with SKU = "1" 
        /// the resulting product variant SKU would be generated as "p-1"
        /// </para>
        /// </summary>
        string DefaultSkuSeparator { get; }
    }
}