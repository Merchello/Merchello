using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the address repository
    /// </summary>
    internal interface ICustomerAddressRepository : IRepositoryQueryable<Guid, ICustomerAddress>
    {
    }
}
