using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Defines a GatewayProviderRepository
    /// </summary>
    internal interface IGatewayProviderRepository : IRepositoryQueryable<Guid, IGatewayProvider>
    {
        /// <summary>
        /// Returns a list of GatewayProviders associated with a ship country
        /// </summary>
        /// <param name="shipCountryKey"></param>
        /// <returns></returns>
        IEnumerable<IGatewayProvider> GetGatewayProvidersByShipCountryKey(Guid shipCountryKey);
    }
}