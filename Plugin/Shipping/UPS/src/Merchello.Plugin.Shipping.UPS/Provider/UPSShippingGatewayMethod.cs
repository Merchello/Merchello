using System;
using System.Linq;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Plugin.Shipping.UPS.Provider
{
    [GatewayMethodEditor("UPS Shipping Method Editor", "~/App_Plugins/Merchello.UPS/shippingmethod.html")]
    public class UPSShippingGatewayMethod : ShippingGatewayMethodBase, IUPSShippingGatewayMethod
    {
        private UPSShippingProcessor _processor;
        private UPSType _upsType;
        private IShipMethod _shipMethod;

        public UPSShippingGatewayMethod(IGatewayResource gatewayResource, IShipMethod shipMethod, IShipCountry shipCountry, ExtendedDataCollection providerExtendedData)
            : base(gatewayResource, shipMethod, shipCountry)
        {
            _processor = new UPSShippingProcessor(providerExtendedData.GetProcessorSettings());
            _shipMethod = shipMethod;
        }

        public override Attempt<IShipmentRateQuote> QuoteShipment(IShipment shipment)
        {
            try
            {
                // TODO this should be made configurable
                var visitor = new UPSShipmentLineItemVisitor() { UseOnSalePriceIfOnSale = false };

                shipment.Items.Accept(visitor);

                var province = ShipMethod.Provinces.FirstOrDefault(x => x.Code == shipment.ToRegion);

                var quote = _processor.CalculatePrice(shipment, _shipMethod, visitor.TotalWeight, province);
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

        public enum UPSType
        {
            UPSNextDayAir,
            UPS2ndDayAir,
            UPSGround,
            UPSWorldwideExpress,
            UPSWorldwideExpidited,
            UPSStandard,
            UPS3DaySelect,
            UPSNextDayAirSaver,
            UPSNextDayAirEarlyAM,
            UPSWorldwideExpressPlus,
            UPS2ndDayAirAM,
        }        
    }
}
