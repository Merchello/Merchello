using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Umbraco.Core;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Defines a an abstract gateway ship method
    /// </summary>
    public abstract class ShippingGatewayMethodBase : IShippingGatewayMethod
    {
        private readonly IShipMethod _shipMethod;
        private readonly IGatewayResource _gatewayResource;
        private readonly IShipCountry _shipCountry;

        protected ShippingGatewayMethodBase(IGatewayResource gatewayResource, IShipMethod shipMethod, IShipCountry shipCountry)
        {
            Mandate.ParameterNotNull(gatewayResource, "gatewayResource");
            Mandate.ParameterNotNull(shipMethod, "shipMethod");
            Mandate.ParameterNotNull(shipCountry, "shipCountry");

            _gatewayResource = gatewayResource;
            _shipMethod = shipMethod;
            _shipCountry = shipCountry;
        }

        /// <summary>
        /// Gets the ship method
        /// </summary>
        public IShipMethod ShipMethod
        {
            get { return _shipMethod; }
        }

        /// <summary>
        /// Gets the ship country
        /// </summary>
        public IShipCountry ShipCountry
        {
            get { return _shipCountry; }
        }

        /// <summary>
        /// Gets the gateway resource
        /// </summary>
        public IGatewayResource GatewayResource
        {
            get { return _gatewayResource; }
        }        

        /// <summary>
        /// Adjusts the rate of the quote based on the province Associated with the ShipMethod
        /// </summary>
        /// <param name="baseRate">The base (unadjusted) rate</param>
        /// <param name="province">The <see cref="IShipProvince"/> associated with the ShipMethod</param>
        /// <returns></returns>
        protected decimal AdjustedRate(decimal baseRate, IShipProvince province)
        {
            if (province == null) return baseRate;
            return province.RateAdjustmentType == RateAdjustmentType.Numeric
                       ? baseRate + province.RateAdjustment
                       : baseRate*(1 + (province.RateAdjustment/100));
        }
        
        /// <summary>
        /// Returns a rate quote for a given shipment
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        public abstract Attempt<IShipmentRateQuote> QuoteShipment(IShipment shipment);

    }
}