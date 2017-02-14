namespace Merchello.Core.Configuration.Sections
{
    using System.Collections.Generic;
    using System.Configuration;

    using Merchello.Core.Configuration.Elements;
    using Merchello.Core.Models;

    /// <summary>
    /// Represents a Merchello countries configuration section.
    /// </summary>
    internal class MerchelloCountriesSection : MerchelloSection, IMerchelloCountriesSection
    {
        /// <summary>
        /// Gets the list of countries.
        /// </summary>
        IEnumerable<ICountry> IMerchelloCountriesSection.Countries
        {
            get
            {
                return this.Countries.Countries;
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("countries", IsRequired = true)]
        internal CountriesElement Countries
        {
            get
            {
                return (CountriesElement)this["countries"];
            }
        }
    }
}