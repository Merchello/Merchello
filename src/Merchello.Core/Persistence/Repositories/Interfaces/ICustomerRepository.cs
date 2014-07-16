namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using Merchello.Core.Models;
    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker Interface for the customer repository
    /// </summary>
    internal interface ICustomerRepository : IRepositoryQueryable<Guid, ICustomer>
    {
    }
}
