namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using Models;
    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker interface for the address repository
    /// </summary>
    internal interface IAnonymousCustomerRepository : IRepositoryQueryable<Guid, IAnonymousCustomer>
    {
    }
}
