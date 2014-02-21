using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Taxation.CountryTaxRate
{
    /// <summary>
    /// Defines the CountryTaxRateTaxationGatewayProvider
    /// </summary>
    public interface ICountryTaxRateTaxationGatewayProvider : ITaxationGatewayProvider
    {
        /// <summary>
        /// Attempts to create a <see cref="ITaxMethod"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="countryCode">The two character ISO country code</param>
        ITaxMethod CreateCountryTaxRate(string countryCode);

        /// <summary>
        /// Attempts to create a <see cref="ITaxMethod"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="countryCode">The two character ISO country code</param>
        /// <param name="percentageTaxRate">The tax rate in percentage for the country</param>
        ITaxMethod CreateCountryTaxRate(string countryCode, decimal percentageTaxRate);

        /// <summary>
        /// Saves a single instance of a <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="taxMethod">The <see cref="ITaxMethod"/> to save</param>
        void SaveCountryTaxRate(ITaxMethod taxMethod);

        /// <summary>
        /// Gets a <see cref="ITaxMethod"/> by it's unique 'key' (Guid)
        /// </summary>
        /// <param name="countryCode">The two char ISO country code</param>
        /// <returns><see cref="ITaxMethod"/></returns>
        ITaxMethod GetCountryTaxRateByCountryCode(string countryCode);

        /// <summary>
        /// Gets a collection of all <see cref="ITaxMethod"/> associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="ITaxMethod"/> </returns>
        IEnumerable<ITaxMethod> GetAllCountryTaxRates();
    }
}