using System;
using System.Collections.Generic;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a product variant
    /// </summary>
    public interface IProductVariant : IProductBase
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

        
    }
}