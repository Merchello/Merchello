using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Defines the product variant repository
    /// </summary>
    public interface IProductVariantRepository : IRepositoryQueryable<Guid, IProductVariant>
    {
         
    }
}