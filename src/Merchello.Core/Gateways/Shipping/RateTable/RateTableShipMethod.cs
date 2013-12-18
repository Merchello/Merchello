using System;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Shipping.RateTable
{
    /// <summary>
    /// Defines the rate table ship method
    /// </summary>
    public class RateTableShipMethod : GatewayShipMethodBase, IRateTableShipMethod
    {
        private readonly QuoteType _quoteType;

        public RateTableShipMethod(IGatewayResource gatewayResource, IShipMethod shipMethod)
            : base(gatewayResource, shipMethod)
        {
            RateTable = new ShipRateTable(shipMethod.Key);
            _quoteType = GatewayResource.ServiceCode == "VBW" ? QuoteType.VaryByWeight : QuoteType.PercentTotal;
        }


        public override decimal QuoteShipment(IShipment shipment)
        {            
            throw new NotImplementedException();
        }


        private enum QuoteType
        {
            VaryByWeight,
            PercentTotal
        }

        /// <summary>
        /// Gets the rate table
        /// </summary>
        public IShipRateTable RateTable { get; private set; }
    }
}