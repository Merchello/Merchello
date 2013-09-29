using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the customer registry item repository
    /// </summary>
    public interface ICustomerRegistryItemRepository : IRepositoryQueryable<int, IPurchaseLineItem>
    {
    }
}
