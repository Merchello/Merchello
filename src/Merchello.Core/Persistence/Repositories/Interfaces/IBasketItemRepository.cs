using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the basket item repository
    /// </summary>
    public interface IBasketItemRepository : IRepositoryQueryable<int, IBasketItem>
    {
    }
}
