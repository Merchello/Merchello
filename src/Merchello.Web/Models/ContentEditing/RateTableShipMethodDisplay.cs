using System;
using System.Collections.Generic;

namespace Merchello.Web.Models.ContentEditing
{
    public class RateTableShipMethodDisplay
    {
        public ShipMethodDisplay ShipMethod { get; set; }
        //public ShipMethodDisplay GatewayResource { get; set; }
        public ShipRateTableDisplay RateTable { get; set; }
    }
}
