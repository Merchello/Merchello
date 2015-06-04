namespace Merchello.Core.Marketing.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Merchello.Core.Models;

    using umbraco.cms.businesslogic.packager;

    /// <summary>
    /// The product constraint data.
    /// </summary>
    /// <remarks>
    /// Used when working with data stored in the <see cref="RestrictToProductSelectionConstraint"/>
    /// </remarks>
    internal class ProductConstraintData
    {
        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        public Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the collection of product variant keys.
        /// </summary>
        public IEnumerable<Guid> VariantKeys { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether specific variants were specified.
        /// </summary>
        public bool SpecifiedVariants { get; set; }
    }

    internal static class ProductConstraintDataExtensions
    {
        public static string GetUiDisplayText(this IEnumerable<ProductConstraintData> constraints)
        {
            return constraints.GetUiDisplayText(MerchelloContext.Current);
        }

        public static string GetUiDisplayText(this IEnumerable<ProductConstraintData> constraints, IMerchelloContext mc)
        {
            if (mc == null)
            {
                return string.Empty;
            }

            var productService = mc.Services.ProductService;
            var constraintsArray = constraints as ProductConstraintData[] ?? constraints.ToArray();
            var products = productService.GetByKeys(constraintsArray.Select(x => x.ProductKey));

            var productNames = string.Join(", ", products.Select(x => x.Name));
            if (constraintsArray.Any(x => x.SpecifiedVariants))
            {
                productNames += " - with specific variants";
            }

            return productNames;
        }   
    }
}