namespace Merchello.Core.Configuration.Outline
{
    using System.Configuration;

    /// <summary>
    /// The currency format element.
    /// </summary>
    public class CurrencyFormatElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the currency 'code' (key) for the currency format collection element.
        /// </summary>
        [ConfigurationProperty("currencyCode", IsKey = true)]
        public string CurrencyCode
        {
            get { return (string)this["currencyCode"]; }
            set { this["currencyCode"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the region requires a postal code.
        /// </summary>
        [ConfigurationProperty("format", DefaultValue = null, IsRequired = false)]
        public string Format
        {
            get { return (string)this["format"]; }
            set { this["format"] = value; }
        }
    }
}