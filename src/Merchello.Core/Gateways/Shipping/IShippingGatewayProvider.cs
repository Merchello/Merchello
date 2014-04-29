using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Defines the base shipping gateway provider
    /// </summary>
    public interface IShippingGatewayProvider : IProvider
    {

        /// <summary>
        /// Creates an instance of a ship method (T) without persisting it to the database
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// ShipMethods should be unique with respect to <see cref="IShipCountry"/> and <see cref="IGatewayResource"/>
        /// 
        /// </remarks>
        IShippingGatewayMethod CreateShippingGatewayMethod(IGatewayResource gatewayResource, IShipCountry shipCountry, string name);

        /// <summary>
        /// Saves a shipmethod
        /// </summary>
        /// <param name="shippingGatewayMethod"></param>
        void SaveShippingGatewayMethod(IShippingGatewayMethod shippingGatewayMethod);

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGatewayResource> ListResourcesOffered();

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        IEnumerable<IShippingGatewayMethod> GetAllShippingGatewayMethods(IShipCountry shipCountry);

        /// <summary>
        /// Gets a <see cref="IShippingGatewayMethod"/> by it's <see cref="IShipMethod"/> key
        /// </summary>
        /// <param name="shipMethodKey">The <see cref="IShipMethod"/> key</param>
        /// <returns>The <see cref="IShippingGatewayMethod"/></returns>
        IShippingGatewayMethod GetShippingGatewayMethod(Guid shipMethodKey);

        /// <summary>
        /// Deletes an Active ShipMethod
        /// </summary>
        /// <param name="shippingGatewayMethod"></param>
        void DeleteShippingGatewayMethod(IShippingGatewayMethod shippingGatewayMethod);

        /// <summary>
        /// Returns a collection of available <see cref="IShippingGatewayMethod"/> associated by this provider for a given shipment
        /// </summary>
        /// <param name="shipment"><see cref="IShipment"/></param>
        /// <returns>A collection of <see cref="IShippingGatewayMethod"/></returns>
        IEnumerable<IShippingGatewayMethod> GetShippingGatewayMethodsForShipment(IShipment shipment);

        /// <summary>
        /// Returns a collection of all available <see cref="IShipmentRateQuote"/> for a given shipment
        /// </summary>
        /// <param name="shipment"><see cref="IShipmentRateQuote"/></param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        IEnumerable<IShipmentRateQuote> QuoteShippingGatewayMethodsForShipment(IShipment shipment);

        /// <summary>
        /// Returns a collection of all available <see cref="IShipmentRateQuote"/> for a given shipment
        /// </summary>        
        /// <param name="strategy">The quotation strategy</param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        IEnumerable<IShipmentRateQuote> QuoteShippingGatewayMethodsForShipment(ShipmentRateQuoteStrategyBase strategy);

        /// <summary>
        /// Returns a collection of all available <see cref="IShipMethod"/>s assoicated with this provider
        /// </summary>
        IEnumerable<IShipMethod> ShipMethods { get; } 
    }
}