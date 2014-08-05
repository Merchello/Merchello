namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Core.Gateways.Shipping.FixedRate;

    /// <summary>
    /// The ship fixed rate table display.
    /// </summary>
    public class ShipFixedRateTableDisplay
    {
        /// <summary>
        /// Gets or sets the ship method key.
        /// </summary>
        public Guid ShipMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the ship country key.
        /// </summary>
        public Guid ShipCountryKey { get; set; }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        public IEnumerable<ShipRateTierDisplay> Rows { get; set; }
    }

        #region Utility Extensions

    /// <summary>
    /// The ship fixed rate table display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class ShipFixedRateTableDisplayExtensions
    {
        /// <summary>
        /// The to fixed rate ship method.
        /// </summary>
        /// <param name="rateTable">
        /// The rate table.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// The <see cref="IFixedRateShippingGatewayMethod"/>.
        /// </returns>
        internal static IFixedRateShippingGatewayMethod ToFixedRateShipMethod(this ShipFixedRateTableDisplay rateTable, IFixedRateShippingGatewayMethod destination)
        {
            // Rate table
            var existingRows = rateTable.Rows.Where(x => !x.Key.Equals(Guid.Empty)).ToArray();
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
                    row =>
                        !row.Key.Equals(Guid.Empty) &&
                        existingRows.All(x => x.Key != row.Key && !x.Key.Equals(Guid.Empty)));

            foreach (var remove in removers)
            {
                destination.RateTable.DeleteRow(remove);
            }

            // add any new rows
            foreach (var newRow in rateTable.Rows.Where(x => x.Key == Guid.Empty))
            {
                destination.RateTable.AddRow(newRow.RangeLow, newRow.RangeHigh, newRow.Rate);
            }

            return destination;
        }
    }

        #endregion
}
