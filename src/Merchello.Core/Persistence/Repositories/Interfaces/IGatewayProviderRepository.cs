using System;
using Merchello.Core.Models.Interfaces;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Defines a GatewayProviderRepository
    /// </summary>
    internal interface IGatewayProviderRepository : IRepositoryQueryable<Guid, IGatewayProvider>
    {
         
    }
}