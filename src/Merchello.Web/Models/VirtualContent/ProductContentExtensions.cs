namespace Merchello.Web.Models.VirtualContent
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.ContentEditing.Collections;

    /// <summary>
    /// Extension methods for <see cref="IProductContent"/>.
    /// </summary>
    public static class ProductContentExtensions
    {
        /// <summary>
        /// Specifies the model culture setting.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        public static void SpecifyCulture(this IProductContent content, CultureInfo culture)
        {
            content.SpecifyCulture(culture.Name);
        }

        /// <summary>
        /// Gets the <see cref="ProductDisplay"/> from <see cref="IProductContent"/>.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        public static ProductDisplay AsProductDisplay(this IProductContent content)
        {
            return ((ProductContent)content).ProductDisplay;
        }

        /// <summary>
        /// Gets the <see cref="ProductVariantDisplay"/> from <see cref="IProductVariantContent"/>.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        public static ProductVariantDisplay AsProductVariantDisplay(this IProductVariantContent content)
        {
            return ((ProductVariantContent)content).ProductVariantDisplay;
        }

        /// <summary>
        /// The current effective price.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The sale price if on sale, otherwise returns the price.
        /// </returns>
        public static decimal PriceOrSalePrice(this IProductContentBase content)
        {
            return content.OnSale ? content.SalePrice : content.Price;
        }

        /// <summary>
        /// Returns static collections containing the product.
        /// </summary>
        /// <param name="product">
        /// The <see cref="IProductContent"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{EntityCollectionDisplay}"/>.
        /// </returns>
        public static IEnumerable<EntityCollectionDisplay> GetCollectionsContaining(this IProductContent product)
        {
            if (!MerchelloContext.HasCurrent) return Enumerable.Empty<EntityCollectionDisplay>();
            return
                ((EntityCollectionService)MerchelloContext.Current.Services.EntityCollectionService)
                    .GetEntityCollectionsByProductKey(product.Key).Select(x => x.ToEntityCollectionDisplay());
        }
    }
}