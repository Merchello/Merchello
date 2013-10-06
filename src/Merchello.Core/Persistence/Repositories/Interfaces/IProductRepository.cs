using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the address repository
    /// </summary>
    public interface IProductRepository : IRepositoryQueryable<Guid, IProduct>
    {
        /// <summary>
        /// True/false indicating whether or not a sku is already exists in the database
        /// </summary>
        /// <param name="sku">The sku to be tested</param>
        /// <returns></returns>
        bool SkuExists(string sku);

        ///// <summary>
        ///// Saves a product option associated with a product
        ///// </summary>
        ///// <param name="product">The <see cref="IProduct"/> with the option to be saved</param>
        ///// <param name="productOption">The <see cref="IProductOption"/> to be saved</param>
        //void SaveProductOption(IProduct product, IProductOption productOption);
    }
}
