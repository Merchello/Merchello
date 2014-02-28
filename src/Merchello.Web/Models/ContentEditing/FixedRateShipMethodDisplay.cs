using Merchello.Core.Gateways.Shipping.FixedRate;
using System;
using System.Collections.Generic;

namespace Merchello.Web.Models.ContentEditing
{
    public class FixedRateShipMethodDisplay
    {
        public ShipMethodDisplay ShipMethod { get; set; }
        public GatewayResourceDisplay GatewayResource { get; set; }
        public ShipFixedRateTableDisplay RateTable { get; set; }
        public FixedRateShippingGatewayMethod.QuoteType RateTableType { get; set; }
    }
}
