
using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Respositories
{
    /// <summary>
    /// Marker interface for customer repositories
    /// </summary>
    public interface ICustomerRepository : IRepository<Guid, ICustomer>
    {
    }
}
