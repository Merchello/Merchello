using System;
using System.Globalization;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Umbraco.Core;

namespace Merchello.Core.Gateways.Shipping.RateTable
{
    /// <summary>
    /// Defines the rate table ship method
    /// </summary>
    public class RateTableShipMethod : GatewayShipMethodBase, IRateTableShipMethod
    {
        private readonly QuoteType _quoteType;

        public RateTableShipMethod(IGatewayResource gatewayResource, IShipMethod shipMethod, IShipCountry shipCountry)
            :this(gatewayResource, shipMethod, shipCountry, new ShipRateTable(shipMethod.Key))
        {}

        public RateTableShipMethod(IGatewayResource gatewayResource, IShipMethod shipMethod, IShipCountry shipCountry, IShipRateTable rateTable)
            : base(gatewayResource, shipMethod, shipCountry)
        {
            RateTable = new ShipRateTable(shipMethod.Key);
            _quoteType = GatewayResource.ServiceCode == RateTableShippingGatewayProvider.VaryByWeightPrefix ? QuoteType.VaryByWeight : QuoteType.PercentTotal;
            RateTable = rateTable;
        }

        public override Attempt<IShipmentRateQuote> QuoteShipment(IShipment shipment)
        {
            // TODO this should be made configurable
            var visitor = new RateTableShipMethodShipmentLineItemVisitor { UseOnSalePriceIfOnSale = false };

            shipment.Items.Accept(visitor);

            var province = ShipMethod.Provinces.FirstOrDefault(x => x.Code == shipment.ToRegion);

            return _quoteType == QuoteType.VaryByWeight
                ? CalculateVaryByWeight(shipment, visitor.TotalWeight, province)
                : CalculatePercentTotal(shipment, visitor.TotalPrice, province);
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

                return Attempt<IShipmentRateQuote>.Succeed(new ShipmentRateQuote(shipment, ShipMethod) {Rate = AdjustedRate(tier.Rate, province)});
        }

        /// <summary>
        /// Calculates the rate based on the percentage of the total shipment item price
        /// </summary>
        /// <param name="shipment">The associated <see cref="IShipment"/></param>
        /// <param name="totalPrice">The total price of the items in the shipment</param>
        /// <param name="province">The <see cref="IShipProvince"/> associated with the shipment destination.  Used for rate adjustments</param>
        /// <returns>Returns an <see cref="Attempt"/> to quote a rate using 'this' ship method</returns>
        private Attempt<IShipmentRateQuote> CalculatePercentTotal(IShipment shipment, decimal totalPrice, IShipProvince province = null)
        {
            var tier = RateTable.Rows.FirstOrDefault(x => x.RangeLow <= totalPrice && totalPrice < x.RangeHigh);
            if (tier == null)
                return
                    Attempt<IShipmentRateQuote>.Fail(
                        new IndexOutOfRangeException("The shipments total weight was calculated to be : " +
                                                     totalPrice.ToString(CultureInfo.InvariantCulture) +
                                                     " which is outside any rate tier defined by the current rate table."));


            return Attempt<IShipmentRateQuote>.Succeed(new ShipmentRateQuote(shipment, ShipMethod) { Rate = AdjustedRate((tier.Rate * .01M) * totalPrice, province)  });
        }

        public enum QuoteType
        {
            VaryByWeight,
            PercentTotal
        }

        /// <summary>
        /// Gets the rate table
        /// </summary>
        public IShipRateTable RateTable { get; private set; }
  
    }
}