namespace Merchello.Core.Persistence.Repositories
{
    using Models;
    using Models.Rdbms;

    /// <summary>
    /// Maker interface for the OrderRepository
    /// </summary>
    internal interface IOrderRepository : IPagedRepository<IOrder, OrderDto>, IAssertsMaxDocumentNumber
    {
    }
}