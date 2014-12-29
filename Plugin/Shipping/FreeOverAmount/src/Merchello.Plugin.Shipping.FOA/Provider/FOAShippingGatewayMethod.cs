namespace Merchello.Plugin.Shipping.FOA.Provider
{
    using System;
    using Core.Gateways;
    using Core.Gateways.Shipping;
    using Core.Models;
    using Models;
    using Umbraco.Core;

    /// <summary>
    /// Represents a shipping gateway method returns free over a certain amount.
    /// </summary>
    [GatewayMethodEditor("Free Over Amount Shipping Method Editor", "~/App_Plugins/Merchello.FOA/shippingmethod.html")]
    public class FoaShippingGatewayMethod : ShippingGatewayMethodBase, IFoaShippingGatewayMethod
    {
        /// <summary>
        /// The ship method.
        /// </summary>
        private IShipMethod _shipMethod;

        /// <summary>
        /// The _processor settings.
        /// </summary>
        private readonly FoaProcessorSettings _processorSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="FoaShippingGatewayMethod"/> class.
        /// </summary>
        /// <param name="gatewayResource">
        /// The gateway resource.
        /// </param>
        /// <param name="shipMethod">
        /// The ship method.
        /// </param>
        /// <param name="shipCountry">
        /// The ship country.
        /// </param>
        /// <param name="gatewayProviderSettings">
        /// The gateway provider settings.
        /// </param>
        public FoaShippingGatewayMethod(IGatewayResource gatewayResource, IShipMethod shipMethod, IShipCountry shipCountry, IGatewayProviderSettings gatewayProviderSettings) : 
            base(gatewayResource, shipMethod, shipCountry)
        {
            _processorSettings = gatewayProviderSettings.ExtendedData.GetProcessorSettings();

            _shipMethod = shipMethod;
        }

        public override Attempt<IShipmentRateQuote> QuoteShipment(IShipment shipment)
        {
            try
            {
                var visitor = new FoaShipmentLineItemVisitor() { UseOnSalePriceIfOnSale = false };

                shipment.Items.Accept(visitor);

                if (visitor.TotalPrice >= _processorSettings.Amount)
                {
                    return Attempt<IShipmentRateQuote>.Succeed(new ShipmentRateQuote(shipment, ShipMethod) { Rate = 0 });
                }
                else
                {
                    // TODO shouldn't this just fail so that a different method is selected or have a configurable default rate. 
                    return Attempt<IShipmentRateQuote>.Succeed(new ShipmentRateQuote(shipment, ShipMethod) { Rate = 100 });
                }
            }
            catch (Exception ex)
            {
                return Attempt<IShipmentRateQuote>.Fail(
                           new Exception("An error occured during your request : " +
                                                        ex.Message +
                                                        " Please contact your administrator or try again."));
            }
        }
    }
}
