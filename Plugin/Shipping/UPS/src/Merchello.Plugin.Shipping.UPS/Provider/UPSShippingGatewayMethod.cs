using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Shipping.FixedRate;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Services;
using Umbraco.Core;

namespace Merchello.Plugin.Shipping.UPS.Provider
{
    public class UPSShippingGatewayMethod : ShippingGatewayMethodBase, IUPSShippingGatewayMethod
    {
        private UPSShippingProcessor _processor;
        private QuoteType _quoteType;
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

        public enum QuoteType
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
        
        /// <summary>
        /// Gets the rate table
        /// </summary>
        public IUPSShippingRateTable RateTable { get; private set; }

        /// <summary>
        /// Gets the quote type
        /// </summary>
        public QuoteType RateTableType
        {
            get { return _quoteType; }
        }
    }
}
