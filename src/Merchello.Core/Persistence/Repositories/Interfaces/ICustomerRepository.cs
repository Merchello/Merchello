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
        /// <summary>
        /// Returns a <see cref="ICustomer"/> by it's entity key
        /// </summary>
        /// <param name="entityKey">The GUID entity key</param>
        /// <returns>The <see cref="ICustomer"/></returns>
        ICustomer GetByEntityKey(Guid entityKey);
    }
}
