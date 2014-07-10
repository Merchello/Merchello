using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Plugin.Shipping.FOA.Provider
{

    [GatewayMethodEditor("Free Over Amount Shipping Method Editor", "~/App_Plugins/Merchello.FOA/shippingmethod.html")]
    public class FoaShippingGatewayMethod : ShippingGatewayMethodBase, IFoaShippingGatewayMethod
    {
        private IShipMethod _shipMethod;
        private readonly ExtendedDataCollection _extendedDataCollection;
        public FoaShippingGatewayMethod(IGatewayResource gatewayResource, IShipMethod shipMethod, IShipCountry shipCountry, ExtendedDataCollection providerExtendedData) : 
            base(gatewayResource, shipMethod, shipCountry)
        {
            _extendedDataCollection = providerExtendedData;
            _shipMethod = shipMethod;
        }

        public override Attempt<IShipmentRateQuote> QuoteShipment(IShipment shipment)
        {
            try
            {
                var visitor = new FoaShipmentLineItemVisitor() { UseOnSalePriceIfOnSale = false };

                shipment.Items.Accept(visitor);

                if (visitor.TotalPrice <= _extendedDataCollection.GetProcessorSettings().Amount)
                {
                    return Attempt<IShipmentRateQuote>.Succeed(new ShipmentRateQuote(shipment, ShipMethod) { Rate = 0 });
                }
                else
                {
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
