using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Defines the base shipping gateway provider
    /// </summary>
    public interface IShippingGatewayProvider : IGateway
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
        IGatewayShipMethod CreateShipMethod(IGatewayResource gatewayResource, IShipCountry shipCountry, string name);

        /// <summary>
        /// Saves a shipmethod
        /// </summary>
        /// <param name="gatewayShipMethod"></param>
        void SaveShipMethod(IGatewayShipMethod gatewayShipMethod);

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGatewayResource> ListResourcesOffered();

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGatewayShipMethod> GetActiveShipMethods(IShipCountry shipCountry);

        /// <summary>
        /// Deletes an Active ShipMethod
        /// </summary>
        /// <param name="gatewayShipMethod"></param>
        void DeleteShipMethod(IGatewayShipMethod gatewayShipMethod);

        /// <summary>
        /// Returns a collection of available <see cref="IGatewayShipMethod"/> associated by this provider for a given shipment
        /// </summary>
        /// <param name="shipment"><see cref="IShipment"/></param>
        /// <returns>A collection of <see cref="IGatewayShipMethod"/></returns>
        IEnumerable<IGatewayShipMethod> GetAvailableShipMethodsForShipment(IShipment shipment);

        /// <summary>
        /// Returns a collection of all available <see cref="IShipmentRateQuote"/> for a given shipment
        /// </summary>
        /// <param name="shipment"><see cref="IShipmentRateQuote"/></param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        IEnumerable<IShipmentRateQuote> QuoteAvailableShipMethodsForShipment(IShipment shipment);

        /// <summary>
        /// Returns a collection of all available <see cref="IShipmentRateQuote"/> for a given shipment
        /// </summary>        
        /// <param name="strategy">The quotation strategy</param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        IEnumerable<IShipmentRateQuote> QuoteAvailableShipMethodsForShipment(ShipmentRateQuoteStrategyBase strategy);

        /// <summary>
        /// Returns a collection of all available <see cref="IShipMethod"/>s assoicated with this provider
        /// </summary>
        IEnumerable<IShipMethod> ShipMethods { get; } 
    }
}