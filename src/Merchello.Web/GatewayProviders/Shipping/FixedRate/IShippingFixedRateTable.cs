using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Web.GatewayProviders.Shipping.FixedRate
{
    public interface IShippingFixedRateTable
    {
        /// <summary>
        /// The associated ShipMethodKey
        /// </summary>
        Guid ShipMethodKey { get; }

        /// <summary>
        /// Adds a rate tier row to the ship rate table
        /// </summary>
        /// <param name="rangeLow">The lowest qualifying value defining the range</param>
        /// <param name="rangeHigh">The highest qualifying value defining the range</param>
        /// <param name="rate">The rate or cost assoicated with the range</param>
        void AddRow(decimal rangeLow, decimal rangeHigh, decimal rate);

        ///// <summary>
        ///// Updates an existing <see cref="IShipRateTier"/> in the <see cref="IShipRateTable"/>
        ///// </summary>
        ///// <param name="shipRateTier">The <see cref="IShipRateTier"/> to update</param>
        //void UpdateRow(IShipRateTier shipRateTier);

        /// <summary>
        /// Deletes a rate tier row from the ship rate table
        /// </summary>
        /// <param name="shipRateTier"></param>
        void DeleteRow(IShipRateTier shipRateTier);

        /// <summary>
        /// Saves the rate table to the database
        /// </summary>
        void Save();

        /// <summary>
        /// Gets the decimal rate associated with the range
        /// </summary>
        /// <param name="rangeValue">The value within a range used to determine which rate to return</param>
        /// <returns>A decimal rate</returns>
        decimal FindRate(decimal rangeValue);

        /// <summary>
        /// The rows of the rate table
        /// </summary>
        IEnumerable<IShipRateTier> Rows { get; }
    }
}