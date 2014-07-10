namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Core.Gateways.Shipping.FixedRate;

    /// <summary>
    /// The rate table ship method display.
    /// </summary>
    public class RateTableShipMethodDisplay : ShipMethodDisplay
    {
        /// <summary>
        /// Gets or sets the rate table.
        /// </summary>
        public ShipFixedRateTableDisplay RateTable { get; set; }

        /// <summary>
        /// Gets or sets the rate table type.
        /// </summary>
        public FixedRateShippingGatewayMethod.QuoteType RateTableType { get; set; }
    }


    #region Utility Extensions

    /// <summary>
    /// The rate table ship method extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class RateTableShipMethodExtensions
    {
        /// <summary>
        /// The to fixed rate ship method.
        /// </summary>
        /// <param name="rateTableShipMethod">
        /// The rate table ship method.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// The <see cref="IFixedRateShippingGatewayMethod"/>.
        /// </returns>
        internal static IFixedRateShippingGatewayMethod ToFixedRateShipMethod(this RateTableShipMethodDisplay rateTableShipMethod, IFixedRateShippingGatewayMethod destination)
        {            
            // Rate table

            var existingRows = rateTableShipMethod.RateTable.Rows.Where(x => !x.Key.Equals(Guid.Empty)).ToArray();
            foreach (var mapRow in existingRows)
            {
                var row = destination.RateTable.Rows.FirstOrDefault(x => x.Key == mapRow.Key);
                if (row != null)
                {
                    row.Rate = mapRow.Rate;
                }
            }

            // remove existing rows that previously existed but were deleted in the UI
            var removers =
                destination.RateTable.Rows.Where(
                    row => !row.Key.Equals(Guid.Empty) && existingRows.All(x => x.Key != row.Key && !x.Key.Equals(Guid.Empty)));

            foreach (var remove in removers)
            {
                destination.RateTable.DeleteRow(remove);
            }

            // add any new rows
            foreach (var newRow in rateTableShipMethod.RateTable.Rows.Where(x => x.Key == Guid.Empty))
            {
                destination.RateTable.AddRow(newRow.RangeLow, newRow.RangeHigh, newRow.Rate);
            }

            return destination;
        }

    #endregion
    }
}