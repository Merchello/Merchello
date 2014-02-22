using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Taxation.FixedRate
{
    /// <summary>
    /// Defines the FixedRateTaxationGatewayProvider
    /// </summary>
    public interface IFixedRateTaxationGatewayProvider : ITaxationGatewayProvider
    {
        
        /// <summary>
        /// Gets a <see cref="ITaxMethod"/> by it's unique 'key' (Guid)
        /// </summary>
        /// <param name="countryCode">The two char ISO country code</param>
        /// <returns><see cref="ITaxMethod"/></returns>
        ITaxMethod GetTaxMethodByCountryCode(string countryCode);

        /// <summary>
        /// Gets a collection of all <see cref="ITaxMethod"/> associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="ITaxMethod"/> </returns>
        IEnumerable<ITaxMethod> GetAllTaxMethods();
    }
}