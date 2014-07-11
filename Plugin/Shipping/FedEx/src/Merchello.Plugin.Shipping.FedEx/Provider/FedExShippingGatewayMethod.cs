using System;
using System.Linq;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Plugin.Shipping.FedEx.Provider
{
    [GatewayMethodEditor("FedEx Shipping Method Editor", "~/App_Plugins/Merchello.FedEx/shippingmethod.html")]
    public class FedExShippingGatewayMethod : ShippingGatewayMethodBase, IFedExShippingGatewayMethod
    {
        private FedExShippingProcessor _processor;
        private FedExType _fedExType;
        private IShipMethod _shipMethod;

        public FedExShippingGatewayMethod(IGatewayResource gatewayResource, IShipMethod shipMethod, IShipCountry shipCountry, IGatewayProviderSettings gatewayProviderSettings)
            : base(gatewayResource, shipMethod, shipCountry)
        {
            _processor = new FedExShippingProcessor(gatewayProviderSettings.ExtendedData.GetProcessorSettings());
            _shipMethod = shipMethod;
        }

        public override Attempt<IShipmentRateQuote> QuoteShipment(IShipment shipment)
        {
            try
            {
                // TODO this should be made configurable
                var visitor = new FedExShipmentLineItemVisitor() { UseOnSalePriceIfOnSale = false };

                shipment.Items.Accept(visitor);

                var province = ShipMethod.Provinces.FirstOrDefault(x => x.Code == shipment.ToRegion);

                var quote = _processor.CalculatePrice(shipment, _shipMethod, visitor.TotalWeight, visitor.Quantity, province);
                quote.Result.Rate = AdjustedRate(quote.Result.Rate, province);
                return quote;
            }
            catch (Exception ex)
            {
                return Attempt<IShipmentRateQuote>.Fail(
                           new Exception("An error occured during your request : " +
                                                        ex.Message +
                                                        " Please contact your administrator or try again."));
            }
        }

        public enum FedExType
        {
            FedExSameDay = 1,
            FedExFirstOvernight = 2,
            FedExPriorityOvernight = 3,
            FedExStandardOvernight = 4,
            FedEx2DayAm = 30,
            FedEx2Day = 5,
            FedExExpressSaver = 6,
            FedExHawaiiNeighborIsland = 7,
            FedExGround = 8,
            FedexHomeDelivery = 9,
            FedExIntlNextFlight = 10,
            FedExIntlFirst = 11,
            FedExIntlPriority = 12,
            FedExIntlEconomy = 13,
            FedExGroundUsToCanada = 26,
            FedExUsToPuertoRico = 18,
            FedExExpressFreightUs = 19,
            FedExExpressIntlFreight = 25,
            FedExIntlPremiumSm = 23
        }        
    }
}
