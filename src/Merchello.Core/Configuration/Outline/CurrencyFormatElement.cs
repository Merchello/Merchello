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
        /// Gets or sets a format of the currency ex: {0}{1:0.00}
        /// </summary>
        [ConfigurationProperty("format", DefaultValue = null, IsRequired = false)]
        public string Format
        {
            get { return (string)this["format"]; }
            set { this["format"] = value; }
        }

        /// <summary>
        /// Gets or sets a currency symbol.
        /// </summary>
        [ConfigurationProperty("symbol", DefaultValue = null, IsRequired = false)]
        public string Symbol
        {
            get { return (string)this["symbol"]; }
            set { this["symbol"] = value; }
        }
    }
}