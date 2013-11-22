using System.Collections.Generic;
using System.Globalization;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the SettingsService, which provides access to operations involving configurable Merchello configurations and settings
    /// </summary>
    public interface ISettingsService : IService
    {
        /// <summary>
        /// Returns the <see cref="ICountry"/> for the country code passed.
        /// </summary>
        /// <param name="countryCode">The two letter ISO Region code (country code)</param>
        /// <returns><see cref="ICountry"/> for the country corresponding the the country code passed</returns>
        ICountry GetCountryByCode(string countryCode);

        /// <summary>
        /// Returns a Region collection for all countries
        /// </summary>
        /// <returns>A collection of <see cref="ICountry"/></returns>
        IEnumerable<ICountry> GetAllCountries();

        /// <summary>
        /// Returns a <see cref="ICountry"/> collection for all countries excluding codes passed
        /// </summary>
        /// <param name="excludeCountryCodes">A collection of country codes to exclude from the result set</param>
        /// <returns>A collection of <see cref="ICountry"/></returns>
        IEnumerable<ICountry> GetAllCountries(string[] excludeCountryCodes);

    }
}