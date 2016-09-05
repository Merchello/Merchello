namespace Merchello.Core.Configuration.Outline
{
    using System.Configuration;

    /// <summary>
    /// The route element.
    /// </summary>
    public class RouteElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the culture name (key) that is expected on the respective route.
        /// </summary>
        [ConfigurationProperty("cultureName", IsKey = true)]
        public string CultureName
        {
            get { return (string)this["cultureName"]; }
            set { this["cultureName"] = value; }
        }

        /// <summary>
        /// Gets or sets the product slug prefix.
        /// </summary>
        [ConfigurationProperty("productSlugPrefix", IsRequired = true)]
        public string ProductSlugPrefix
        {
            get { return (string)this["productSlugPrefix"]; }
            set { this["productSlugPrefix"] = value; }
        }
    }
}