namespace Merchello.Core.Gateways.Shipping
{
    using System.Collections.Generic;
    using Merchello.Core.Models;

    /// <summary>
    /// Defines a shipping context
    /// </summary>
    public interface IShippingContext : IGatewayProviderTypedContextBase<ShippingGatewayProviderBase>
    {
        /// <summary>
        /// Returns a collection of all <see cref="IShipmentRateQuote"/> that are available for the <see cref="IShipment"/>
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/> to quote</param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        IEnumerable<IShipmentRateQuote> GetShipRateQuotesForShipment(IShipment shipment);

        /// <summary>
        /// Returns a list of all countries that can be assigned to a shipment
        /// </summary>
        /// <returns>A collection of all <see cref="ICountry"/> that have been identified as allowable shipping destinations</returns>
        IEnumerable<ICountry> GetAllowedShipmentDestinationCountries();

        /// <summary>
        /// Gets a collection of <see cref="ShippingGatewayProviderBase"/> by ship country
        /// </summary>
        /// <param name="shipCountry">The <see cref="IShipCountry"/></param>
        /// <returns>A collection of <see cref="IShippingGatewayProvider"/>s associated with the ship country</returns>
        IEnumerable<IShippingGatewayProvider> GetGatewayProvidersByShipCountry(IShipCountry shipCountry);
    }
}