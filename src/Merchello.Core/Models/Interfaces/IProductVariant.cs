﻿namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a product variant
    /// </summary>
    public interface IProductVariant : IProductBase, IEntity
    {
        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        Guid ProductKey { get; set; }
            
        /// <summary>
        /// Gets the product's attributes
        /// </summary>
        /// <remarks>
        /// Designated options that make up this product
        /// e.g. for product T-Shirt -> attributes could be  Small, Black
        /// </remarks>
        IEnumerable<IProductAttribute> Attributes { get; }

        /// <summary>
        /// Gets the total (sum) of inventory "counts" across all associated warehouses
        /// </summary>
        /// <returns></returns>
        int TotalInventoryCount { get; }
    }
}