using System.Collections.Generic;

namespace Merchello.Core.Models
{
    public interface IProduct : IProductBase
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



        /// <summary>
        /// Associates a product with a warehouse
        /// </summary>
        /// <param name="warehouseId">The 'unique' id of the <see cref="IWarehouse"/></param>
        void AddToWarehouse(int warehouseId);


    }
}