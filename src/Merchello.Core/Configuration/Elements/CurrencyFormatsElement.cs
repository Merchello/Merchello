namespace Merchello.Core.Configuration.Elements
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Acquired.Configuration;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents a configuration element for configuring currency formats.
    /// </summary>
    internal class CurrencyFormatsElement : RawXmlConfigurationElement
    {
        /// <summary>
        /// Creates the collection of configured currency formats.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{ICurrencyFormat}"/>.
        /// </returns>
        public IEnumerable<ICurrencyFormat> GetCurrencyFormats()
        {
            return RawXml.Elements("format")
                            .Select(x => 
                                new CurrencyFormat(
                                    x.Attribute("format").Value, 
                                    x.Attribute("currencyCode").Value)); // REFACTOR-todo currency code needs to be converted to currency symbol
        }
    }
}