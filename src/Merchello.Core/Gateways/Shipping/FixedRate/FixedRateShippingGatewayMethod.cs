namespace Merchello.Core.Gateways.Shipping.FixedRate
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Models;
    using Models.Interfaces;
    using Umbraco.Core;

    /// <summary>
    /// Defines the rate table ship method
    /// </summary>
    [GatewayMethodEditor("Fixed rate ship method editor", "Fixed rate ship method editor", "~/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shipping.fixedratemethod.html")]
    public class FixedRateShippingGatewayMethod : ShippingGatewayMethodBase, IFixedRateShippingGatewayMethod
    {
        /// <summary>
        /// The quote type.
        /// </summary>
        private readonly QuoteType _quoteType;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedRateShippingGatewayMethod"/> class.
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
        public FixedRateShippingGatewayMethod(IGatewayResource gatewayResource, IShipMethod shipMethod, IShipCountry shipCountry)
            : this(gatewayResource, shipMethod, shipCountry, new ShippingFixedRateTable(shipMethod.Key))
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedRateShippingGatewayMethod"/> class.
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
        /// <param name="rateTable">
        /// The rate table.
        /// </param>
        public FixedRateShippingGatewayMethod(IGatewayResource gatewayResource, IShipMethod shipMethod, IShipCountry shipCountry, IShippingFixedRateTable rateTable)
            : base(gatewayResource, shipMethod, shipCountry)
        {
            RateTable = new ShippingFixedRateTable(shipMethod.Key);
            _quoteType = GatewayResource.ServiceCode == FixedRateShippingGatewayProvider.VaryByWeightPrefix ? QuoteType.VaryByWeight : QuoteType.VaryByPrice;
            RateTable = rateTable;
        }

        /// <summary>
        /// The quote type
        /// </summary>
        public enum QuoteType
        {
            /// <summary>
            /// Indicates the quote is based shipment weight
            /// </summary>
            VaryByWeight,

            /// <summary>
            /// Indicates the quote is based on total shipment price
            /// </summary>
            VaryByPrice
        }

        /// <summary>
        /// Gets the rate table
        /// </summary>
        public IShippingFixedRateTable RateTable { get; private set; }

        /// <summary>
        /// Gets the quote type
        /// </summary>
        public QuoteType RateTableType
        {
            get { return _quoteType; }
        }

        /// <summary>
        /// The quote shipment.
        /// </summary>
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<IShipmentRateQuote> QuoteShipment(IShipment shipment)
        {
            // TODO this should be made configurable
            var visitor = new FixedRateShipmentLineItemVisitor { UseOnSalePriceIfOnSale = false };

            shipment.Items.Accept(visitor);

            var province = ShipMethod.Provinces.FirstOrDefault(x => x.Code == shipment.ToRegion);

            return _quoteType == QuoteType.VaryByWeight
                ? CalculateVaryByWeight(shipment, visitor.TotalWeight, province)
                : CalculateVaryByPrice(shipment, visitor.TotalPrice, province);
        }
      
        /// <summary>
        /// Calculates the rate based on the total weight of the items in the shipment
        /// </summary>
        /// <param name="shipment">The associated <see cref="IShipment"/></param>
        /// <param name="totalWeight">The total weight of the items in the shipment</param>
        /// <param name="province">The <see cref="IShipProvince"/> associated with the shipment destination.  Used for rate adjustments</param>
        /// <returns>Returns an <see cref="Attempt"/> to quote a rate using 'this' ship method</returns>
        private Attempt<IShipmentRateQuote> CalculateVaryByWeight(IShipment shipment, decimal totalWeight, IShipProvince province = null)
        {
            var tier = RateTable.Rows.FirstOrDefault(x => x.RangeLow <= totalWeight && totalWeight < x.RangeHigh);
            if (tier == null)
                return
                    Attempt<IShipmentRateQuote>.Fail(
                        new IndexOutOfRangeException("The shipments total weight was calculated to be : " +
                                                     totalWeight.ToString(CultureInfo.InvariantCulture) +
                                                     " which is outside any rate tier defined by the current rate table."));

                return Attempt<IShipmentRateQuote>.Succeed(new ShipmentRateQuote(shipment, ShipMethod) { Rate = AdjustedRate(tier.Rate, province) });
        }

        /// <summary>
        /// Calculates the rate based on the total shipment item price
        /// </summary>
        /// <param name="shipment">The associated <see cref="IShipment"/></param>
        /// <param name="totalPrice">The total price of the items in the shipment</param>
        /// <param name="province">The <see cref="IShipProvince"/> associated with the shipment destination.  Used for rate adjustments</param>
        /// <returns>Returns an <see cref="Attempt"/> to quote a rate using 'this' ship method</returns>
        private Attempt<IShipmentRateQuote> CalculateVaryByPrice(IShipment shipment, decimal totalPrice, IShipProvince province = null)
        {
            var tier = RateTable.Rows.FirstOrDefault(x => x.RangeLow <= totalPrice && totalPrice < x.RangeHigh);
            if (tier == null)
                return
                    Attempt<IShipmentRateQuote>.Fail(
                        new IndexOutOfRangeException("The shipments total weight was calculated to be : " +
                                                     totalPrice.ToString(CultureInfo.InvariantCulture) +
                                                     " which is outside any rate tier defined by the current rate table."));


            return Attempt<IShipmentRateQuote>.Succeed(new ShipmentRateQuote(shipment, ShipMethod) { Rate = AdjustedRate(tier.Rate, province) });
        }
    }
}