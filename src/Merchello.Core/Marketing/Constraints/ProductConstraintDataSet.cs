namespace Merchello.Core.Marketing.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// The product constraint data.
    /// </summary>
    /// <remarks>
    /// Used when working with data stored in the <see cref="ProductSelectionFilterConstraint"/>
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

        /// <summary>
        /// Gets or sets a value indicating whether the product should be excluded.
        /// </summary>
        public bool Exclude { get; set; }
    }

    /// <summary>
    /// The product constraint data extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here.")]
    internal static class ProductConstraintDataExtensions
    {
        /// <summary>
        /// Gets display text for the back office UI.
        /// </summary>
        /// <param name="constraints">
        /// The constraints.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetUiDisplayText(this IEnumerable<ProductConstraintData> constraints)
        {
            return constraints.GetUiDisplayText(MerchelloContext.Current);
        }

        /// <summary>
        /// Gets display text for the back office UI.
        /// </summary>
        /// <param name="constraints">
        /// The constraints.
        /// </param>
        /// <param name="mc">
        /// The mc.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
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