namespace Merchello.Core.Models
{
    using System;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a product.
    /// </summary>
    public interface IProduct : IProductBase, IEntity
    {
        /// <summary>
        /// Gets the product variant key.
        /// </summary>
        Guid ProductVariantKey { get; }

        /// <summary>
        /// Gets a value indicating whether or not this product group defines product options.
        /// e.g. The product has no required options
        /// </summary>
        bool DefinesOptions { get; }

        /// <summary>
        /// Gets or sets the product's collection of options (Attribute selection)
        /// </summary>
        //ProductOptionCollection ProductOptions { get; set; }

        /// <summary>
        /// Gets or sets the product's collection of <see cref="IProductVariant"/>
        /// </summary>
        /// <remarks>
        /// A product variant is the culmination of a product with product option choices selected
        /// </remarks>
        //ProductVariantCollection ProductVariants { get; set; }
    }
}