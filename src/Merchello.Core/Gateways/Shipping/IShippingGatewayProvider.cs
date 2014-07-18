using System;

namespace Merchello.Core.Gateways.Shipping
{
    using System.Collections.Generic;
    using Models;

    /// <summary>
    /// Defines the base shipping gateway provider
    /// </summary>
    public interface IShippingGatewayProvider : IProvider
    {
        /// <summary>
        /// Gets a collection of all available <see cref="IShipMethod"/>s associated with this provider
        /// </summary>
        IEnumerable<IShipMethod> ShipMethods { get; } 

        /// <summary>
        /// Creates an instance of a ship method (T) without persisting it to the database
        /// </summary>
        /// <param name="gatewayResource">
        /// The gateway Resource.
        /// </param>
        /// <param name="shipCountry">
        /// The ship Country.
        /// </param>
        /// <param name="name">
        /// The name of the method
        /// </param>
        /// <returns>
        /// The newly created <see cref="IShippingGatewayMethod"/>
        /// </returns>
        /// <remarks>
        /// ShipMethods should be unique with respect to <see cref="IShipCountry"/> and <see cref="IGatewayResource"/>
        /// </remarks>
        IShippingGatewayMethod CreateShippingGatewayMethod(IGatewayResource gatewayResource, IShipCountry shipCountry, string name);

        /// <summary>
        /// Saves a shipmethod
        /// </summary>
        /// <param name="shippingGatewayMethod">
        /// The <see cref="IShippingGatewayMethod"/> to be saved
        /// </param>
        void SaveShippingGatewayMethod(IShippingGatewayMethod shippingGatewayMethod);

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <param name="shipCountry">
        /// The ship Country.
        /// </param>
        /// <returns>
        /// A collection of all <see cref="IShippingGatewayMethod"/>s for a given <see cref="IShipCountry"/>
        /// </returns>
        IEnumerable<IShippingGatewayMethod> GetAllShippingGatewayMethods(IShipCountry shipCountry);

        /// <summary>
        /// Deletes an Active ShipMethod
        /// </summary>
        /// <param name="shippingGatewayMethod">
        /// The <see cref="IShippingGatewayMethod"/> to be deleted
        /// </param>
        void DeleteShippingGatewayMethod(IShippingGatewayMethod shippingGatewayMethod);

        /// <summary>
        /// Returns a collection of available <see cref="IShippingGatewayMethod"/> associated by this provider for a given shipment
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/> that was shipped using the method attempting to return</param>
        /// <returns>A collection of <see cref="IShippingGatewayMethod"/></returns>
        IEnumerable<IShippingGatewayMethod> GetShippingGatewayMethodsForShipment(IShipment shipment);

        /// <summary>
        /// Returns a collection of all available <see cref="IShipmentRateQuote"/> for a given shipment
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/> to be quoted</param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        IEnumerable<IShipmentRateQuote> QuoteShippingGatewayMethodsForShipment(IShipment shipment);

        /// <summary>
        /// Returns a collection of all available <see cref="IShipmentRateQuote"/> for a given shipment
        /// </summary>        
        /// <param name="strategy">The quotation strategy</param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        IEnumerable<IShipmentRateQuote> QuoteShippingGatewayMethodsForShipment(ShipmentRateQuoteStrategyBase strategy);
    }
}