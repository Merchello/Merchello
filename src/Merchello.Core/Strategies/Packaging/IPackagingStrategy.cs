using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Strategies.Packaging
{
    /// <summary>
    /// Defines a shipment packaging strategy
    /// </summary>
    public interface IPackagingStrategy : IStrategy
    {
        /// <summary>
        /// Creates a collection of shipments
        /// </summary>
        /// <returns>A collection of <see cref="IShipment"/></returns>
        IEnumerable<IShipment> PackageShipments();
    }
}