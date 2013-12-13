using System;
using System.Collections.Generic;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Web.Models
{
    public interface IShipRateTable
    {
        /// <summary>
        /// The associated ShipMethodKey
        /// </summary>
        Guid ShipMethodKey { get; }

        /// <summary>
        /// Adds a rate tier row to the ship rate table
        /// </summary>
        /// <param name="shipRateTier"></param>
        void AddRow(IShipRateTier shipRateTier);

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
        decimal GetRate(decimal rangeValue);

        /// <summary>
        /// The rows of the rate table
        /// </summary>
        IEnumerable<IShipRateTier> Rows { get; }
    }
}