namespace Merchello.Core.Configuration.Sections
{
    /// <summary>
    /// Represents a configuration section used for product related configurations
    /// </summary>
    public interface IProductsSection : IMerchelloConfigurationSection
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