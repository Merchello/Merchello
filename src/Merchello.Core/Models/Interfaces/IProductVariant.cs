using System;
using System.Collections.Generic;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a product variant
    /// </summary>
    public interface IProductVariant : IProductBase, IIdEntity
    {
        /// <summary>
        /// The key for the defining product
        /// </summary>
        Guid ProductKey { get; set; }
            
        /// <summary>
        /// The product's attributes
        /// </summary>
        /// <remarks>
        /// Designated options that make up this product
        /// e.g. for product T-Shirt -> attributes could be  Small, Black
        /// </remarks>
        IEnumerable<IProductAttribute> Attributes { get; }

        /// <summary>
        /// Associates a product variant with a warehouse
        /// </summary>
        /// <param name="warehouseId">The 'unique' id of the <see cref="IWarehouse"/></param>
        void AddToWarehouse(int warehouseId);

        /// <summary>
        /// Returns the total (sum) of inventory "counts" across all associated warehouses
        /// </summary>
        /// <returns></returns>
        int TotalInventoryCount { get; }

    }
}