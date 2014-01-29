using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
            _quoteType = GatewayResource.ServiceCode == "VBW" ? QuoteType.VaryByWeight : QuoteType.PercentTotal;
            RateTable = rateTable;
        }

        public override Attempt<IShipmentRateQuote> QuoteShipment(IShipment shipment)
        {
            // TODO this should be made configurable
            var visitor = new RateTableShipMethodShipmentLineItemVisitor { UseOnSalePriceIfOnSale = false };
            shipment.Items.Accept(visitor);

            var province = ShipMethod.Provinces.FirstOrDefault(x => x.Code == shipment.ToRegion);

            return _quoteType == QuoteType.VaryByWeight
                ? CalculateVaryByWeight(visitor.TotalWeight, province)
                : CalculatePercentTotal(visitor.TotalPrice, province);
        }
      

        private Attempt<IShipmentRateQuote> CalculateVaryByWeight(decimal totalWeight, IShipProvince province = null)
        {
            var tier = RateTable.Rows.FirstOrDefault(x => x.RangeLow <= totalWeight && totalWeight < x.RangeHigh);
            if (tier == null)
                return
                    Attempt<IShipmentRateQuote>.Fail(
                        new IndexOutOfRangeException("The shipments total weight was calculated to be : " +
                                                     totalWeight.ToString(CultureInfo.InvariantCulture) +
                                                     " which is outside any rate tier defined by the current rate table."));

                return Attempt<IShipmentRateQuote>.Succeed(new ShipmentRateQuote(ShipMethod) {Rate = AdjustedRate(tier.Rate, province)});
        }

        /// <summary>
        /// Calculates the rate based on the percentage of the total shipment item price
        /// </summary>
        /// <param name="totalPrice"></param>
        /// <returns></returns>
        private Attempt<IShipmentRateQuote> CalculatePercentTotal(decimal totalPrice, IShipProvince province = null)
        {
            var tier = RateTable.Rows.FirstOrDefault(x => x.RangeLow <= totalPrice && totalPrice < x.RangeHigh);
            if (tier == null)
                return
                    Attempt<IShipmentRateQuote>.Fail(
                        new IndexOutOfRangeException("The shipments total weight was calculated to be : " +
                                                     totalPrice.ToString(CultureInfo.InvariantCulture) +
                                                     " which is outside any rate tier defined by the current rate table."));


            return Attempt<IShipmentRateQuote>.Succeed(new ShipmentRateQuote(ShipMethod) { Rate = AdjustedRate((tier.Rate * .01M) * totalPrice, province)  });
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