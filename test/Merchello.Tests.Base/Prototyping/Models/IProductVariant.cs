using System;

namespace Merchello.Tests.Base.Prototyping.Models
{
    /// <summary>
    /// Defines a product variant
    /// </summary>
    public interface IProductVariant : IProductBase
    {
        /// <summary>
        /// The key for the defining product
        /// </summary>
        Guid ProductKey { get; }

        /// <summary>
        /// The product's attributes
        /// </summary>
        /// <remarks>
        /// Designated options that make up this product
        /// e.g. for product T-Shirt -> attributes could be  Small, Black
        /// </remarks>
        ProductAttributeCollection Attributes { get; }

        
    }
}