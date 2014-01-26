using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the ShipRateTierService
    /// </summary>
    public interface IShipRateTierService : IService
    {
        /// <summary>
        /// Saves a single <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTier">The <see cref="IShipRateTier"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        void Save(IShipRateTier shipRateTier, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTierList">The collection of <see cref="IShipRateTier"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        void Save(IEnumerable<IShipRateTier> shipRateTierList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTier">The <see cref="IShipRateTier"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        void Delete(IShipRateTier shipRateTier, bool raiseEvents = true);

        /// <summary>
        /// Gets a list of <see cref="IShipRateTier"/> objects given a <see cref="IShipMethod"/> key
        /// </summary>
        /// <param name="shipMethodKey">Guid</param>
        /// <returns>A collection of <see cref="IShipRateTier"/></returns>
        IEnumerable<IShipRateTier> GetShipRateTiersByShipMethodKey(Guid shipMethodKey);

    }
}