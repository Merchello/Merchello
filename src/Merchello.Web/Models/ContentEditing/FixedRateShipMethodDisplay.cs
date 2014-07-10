using System;

namespace Merchello.Web.Models.ContentEditing
{
    using Core.Gateways.Shipping.FixedRate;

    /// <summary>
    /// The fixed rate ship method display.
    /// </summary>
    [Obsolete]
    public class FixedRateShipMethodDisplay : DialogEditorDisplayBase
    {
        /// <summary>
        /// Gets or sets the ship method.
        /// </summary>
        public ShipMethodDisplay ShipMethod { get; set; }

        /// <summary>
        /// Gets or sets the gateway resource.
        /// </summary>
        public GatewayResourceDisplay GatewayResource { get; set; }

        /// <summary>
        /// Gets or sets the rate table.
        /// </summary>
        public ShipFixedRateTableDisplay RateTable { get; set; }

        /// <summary>
        /// Gets or sets the rate table type.
        /// </summary>
        public FixedRateShippingGatewayMethod.QuoteType RateTableType { get; set; }
    }
}
