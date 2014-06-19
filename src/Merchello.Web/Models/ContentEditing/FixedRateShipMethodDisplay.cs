namespace Merchello.Web.Models.ContentEditing
{
    using Core.Gateways.Shipping.FixedRate;

    public class FixedRateShipMethodDisplay : DialogEditorDisplayBase
    {
        public ShipMethodDisplay ShipMethod { get; set; }
        public GatewayResourceDisplay GatewayResource { get; set; }
        public ShipFixedRateTableDisplay RateTable { get; set; }
        public FixedRateShippingGatewayMethod.QuoteType RateTableType { get; set; }
    }
}
