namespace Merchello.Core.Configuration.Elements
{
    using System.Configuration;

    using Merchello.Core.Acquired.Configuration;
    using Merchello.Core.Configuration.Sections;

    /// <inheritdoc/>
    internal class ProductsElement : ConfigurationElement, IProductsSection
    {
        /// <summary>
        /// Gets the SKU separator.
        /// </summary>
        string IProductsSection.DefaultSkuSeparator
        {
            get
            {
                return DefaultSkuSeparator;
            }
        }

        /// <summary>
        /// Gets the default SKU separator used when generating product variants based off product options.
        /// <para>
        /// Example: If DefaultSkuSeparator = "-" and given a product with SKU = "p" and option with SKU = "1" 
        /// the resulting product variant SKU would be generated as "p-1"
        /// </para>
        /// </summary>
        [ConfigurationProperty("defaultSkuSeparator", IsRequired = true)]
        internal InnerTextConfigurationElement<string> DefaultSkuSeparator
        {
            get
            {
                return (InnerTextConfigurationElement<string>)this["defaultSkuSeparator"];
            }
        }
    }
}