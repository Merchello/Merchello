using System;
using System.Collections;
using System.Collections.Generic;

namespace Merchello.Core.Models.Interfaces
{
    /// <summary>
    /// Defines a ship region
    /// </summary>
    internal interface IShipCountry : ICountryBase
    {
        /// <summary>
        /// The unique WarehouseKey (Guid)
        /// </summary>
         Guid WarehouseKey { get; }

        /// <summary>
        /// The collection of ship methods associated with warehouse region
        /// </summary>
        GatewayProviderCollection<IShipGatewayProvider> Providers { get; }
    }
}