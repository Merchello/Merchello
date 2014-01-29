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
        /// Attempts to create a <see cref="ICountryTaxRate"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="countryCode">The two character ISO country code</param>
        ICountryTaxRate CreateCountryTaxRate(string countryCode);

        /// <summary>
        /// Attempts to create a <see cref="ICountryTaxRate"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="countryCode">The two character ISO country code</param>
        /// <param name="percentageTaxRate">The tax rate in percentage for the country</param>
        ICountryTaxRate CreateCountryTaxRate(string countryCode, decimal percentageTaxRate);

        /// <summary>
        /// Saves a single instance of a <see cref="ICountryTaxRate"/>
        /// </summary>
        /// <param name="countryTaxRate">The <see cref="ICountryTaxRate"/> to save</param>
        void SaveCountryTaxRate(ICountryTaxRate countryTaxRate);

        /// <summary>
        /// Gets a <see cref="ICountryTaxRate"/> by it's unique 'key' (Guid)
        /// </summary>
        /// <param name="countryCode">The two char ISO country code</param>
        /// <returns><see cref="ICountryTaxRate"/></returns>
        ICountryTaxRate GetCountryTaxRateByCountryCode(string countryCode);

        /// <summary>
        /// Gets a collection of all <see cref="ICountryTaxRate"/> associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="ICountryTaxRate"/> </returns>
        IEnumerable<ICountryTaxRate> GetAllCountryTaxRates();
    }
}