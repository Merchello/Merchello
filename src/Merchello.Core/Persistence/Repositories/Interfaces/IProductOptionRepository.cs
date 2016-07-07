namespace Merchello.Core.Persistence.Repositories
{
    using System;

    using Merchello.Core.Models;

    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Defines a product option repository.
    /// </summary>
    internal interface IProductOptionRepository : IRepositoryQueryable<Guid, IProductOption>
    {
       // IProductOption GetForProduct(Guid key, Guid productKey);
    }
}