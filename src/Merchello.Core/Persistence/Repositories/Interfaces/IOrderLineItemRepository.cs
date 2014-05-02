using Merchello.Core.Models;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface with the Order Line Item Repository
    /// </summary>
    internal interface IOrderLineItemRepository : ILineItemRepositoryBase<IOrderLineItem>
    { }
}