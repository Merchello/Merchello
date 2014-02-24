using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Defines a taxation gateway provider
    /// </summary>
    public interface ITaxationGatewayProvider : IGateway
    {
        /// <summary>
        /// Attempts to create a <see cref="IGatewayTaxMethod"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="countryCode">The two character ISO country code</param>
        IGatewayTaxMethod CreateTaxMethod(string countryCode);

        /// <summary>
        /// Creates a <see cref="IGatewayTaxMethod"/>
        /// </summary>
        /// <param name="countryCode">The two letter ISO Country Code</param>
        /// <param name="taxPercentageRate">The decimal percentage tax rate</param>
        /// <returns>The <see cref="IGatewayTaxMethod"/></returns>
        IGatewayTaxMethod CreateTaxMethod(string countryCode, decimal taxPercentageRate);

        /// <summary>
        /// Saves a <see cref="IGatewayTaxMethod"/>
        /// </summary>
        /// <param name="gatewayTaxMethod">The <see cref="IGatewayTaxMethod"/> to be saved</param>
        void SaveTaxMethod(IGatewayTaxMethod gatewayTaxMethod);

        /// <summary>
        /// Deletes a <see cref="IGatewayTaxMethod"/>
        /// </summary>
        /// <param name="taxMethod">The <see cref="IGatewayTaxMethod"/> to be deleted</param>
        void DeleteTaxMethod(IGatewayTaxMethod taxMethod);

        /// <summary>
        /// Gets a <see cref="IGatewayTaxMethod"/> by it's unique 'key' (Guid)
        /// </summary>
        /// <param name="countryCode">The two char ISO country code</param>
        /// <returns><see cref="IGatewayTaxMethod"/></returns>
        IGatewayTaxMethod GetGatewayTaxMethodByCountryCode(string countryCode);

        /// <summary>
        /// Gets a collection of all <see cref="IGatewayTaxMethod"/> associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="IGatewayTaxMethod"/> </returns>
        IEnumerable<IGatewayTaxMethod> GetAllGatewayTaxMethods();

        /// <summary>
        /// Returns a collection of all available <see cref="ITaxMethod"/>s assoicated with this provider
        /// </summary>
        IEnumerable<ITaxMethod> TaxMethods { get; } 
    }
}