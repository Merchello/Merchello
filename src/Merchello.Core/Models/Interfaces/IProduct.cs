using System.Collections.Generic;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    public interface IProduct : IProductBase, IEntity
    {

        /// <summary>
        /// True/false indicating whether or not this product group defines product options.
        /// e.g. The product has no required options
        /// </summary>
         bool DefinesOptions { get; }

        /// <summary>
        /// The product's collection of options (Attribute selection)
        /// </summary>
        ProductOptionCollection ProductOptions { get; set; }

        /// <summary>
        /// The product's collection of <see cref="IProductVariant"/>
        /// </summary>
        /// <remarks>
        /// A product variant is the culmination of a product with product option choices selected
        /// </remarks>
        ProductVariantCollection ProductVariants { get; set; }

    }
}