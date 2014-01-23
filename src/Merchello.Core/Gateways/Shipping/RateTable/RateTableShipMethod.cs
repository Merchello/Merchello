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
            :this(gatewayResource, shipMethod, new ShipRateTable(shipMethod.Key))
        {}

        public RateTableShipMethod(IGatewayResource gatewayResource, IShipMethod shipMethod, IShipRateTable rateTable)
            : base(gatewayResource, shipMethod)
        {
            RateTable = new ShipRateTable(shipMethod.Key);
            _quoteType = GatewayResource.ServiceCode == "VBW" ? QuoteType.VaryByWeight : QuoteType.PercentTotal;
            RateTable = rateTable;
        }


        public override decimal QuoteShipment(IShipment shipment)
        {
            return _quoteType == QuoteType.VaryByWeight
                ? CalculateVaryByWeight(shipment)
                : CalculatePercentTotal(shipment);
        }


        private decimal CalculateVaryByWeight(IShipment shipment)
        {
            return 0;
        }

        private decimal CalculatePercentTotal(IShipment shipment)
        {
            return 0;
        }

        public enum QuoteType
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