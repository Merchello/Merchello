namespace Merchello.Core.Gateways.Taxation
{
    using System.Collections.Generic;
    using Models;

    /// <summary>
    /// Defines a taxation gateway provider
    /// </summary>
    public interface ITaxationGatewayProvider : IProvider
    {
        /// <summary>
        /// Gets a collection of all available <see cref="ITaxMethod"/>s assoicated with this provider
        /// </summary>
        IEnumerable<ITaxMethod> TaxMethods { get; } 

        /// <summary>
        /// Attempts to create a <see cref="ITaxationGatewayMethod"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="countryCode">
        /// The two character ISO country code
        /// </param>
        /// <returns>
        /// The <see cref="ITaxationGatewayMethod"/>.
        /// </returns>
        ITaxationGatewayMethod CreateTaxMethod(string countryCode);

        /// <summary>
        /// Creates a <see cref="ITaxationGatewayMethod"/>
        /// </summary>
        /// <param name="countryCode">The two letter ISO Country Code</param>
        /// <param name="taxPercentageRate">The decimal percentage tax rate</param>
        /// <returns>The <see cref="ITaxationGatewayMethod"/></returns>
        ITaxationGatewayMethod CreateTaxMethod(string countryCode, decimal taxPercentageRate);

        /// <summary>
        /// Saves a <see cref="ITaxationGatewayMethod"/>
        /// </summary>
        /// <param name="taxationGatewayMethod">The <see cref="ITaxationGatewayMethod"/> to be saved</param>
        void SaveTaxMethod(ITaxationGatewayMethod taxationGatewayMethod);

        /// <summary>
        /// Deletes a <see cref="ITaxationGatewayMethod"/>
        /// </summary>
        /// <param name="method">The <see cref="ITaxationGatewayMethod"/> to be deleted</param>
        void DeleteTaxMethod(ITaxationGatewayMethod method);

        /// <summary>
        /// Gets a <see cref="ITaxationGatewayMethod"/> by it's unique 'key' (Guid)
        /// </summary>
        /// <param name="countryCode">The two char ISO country code</param>
        /// <returns><see cref="ITaxationGatewayMethod"/></returns>
        ITaxationGatewayMethod GetGatewayTaxMethodByCountryCode(string countryCode);

        /// <summary>
        /// Gets a collection of all <see cref="ITaxationGatewayMethod"/> associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="ITaxationGatewayMethod"/> </returns>
        IEnumerable<ITaxationGatewayMethod> GetAllGatewayTaxMethods();
    }
}