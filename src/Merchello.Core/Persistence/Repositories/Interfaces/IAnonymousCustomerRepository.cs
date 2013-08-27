using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the anonymous customer repository
    /// </summary>
    public interface IAnonymousCustomerRepository :  IRepository<Guid, IAnonymousCustomer>
    {

    }
}
