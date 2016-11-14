namespace Merchello.Web.Models.VirtualContent
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Logging;
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

        /// <summary>
        /// Gets the default <see cref="IProductAttributeContent"/> based on default option choices.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariantContent"/>.
        /// </returns>
        public static IProductVariantContent GetDefaultProductVariant(this IProductContent product)
        {
            if (!product.Options.Any()) return null;

            var attKeys = new List<Guid>();
            foreach (var opt in product.Options)
            {
                var defaultChoice = opt.Choices.FirstOrDefault(x => x.IsDefaultChoice);
                if (defaultChoice == null)
                {
                    defaultChoice = opt.Choices.First();
                    MultiLogHelper.Debug(typeof(ProductCollectionExtensions), "Could not find default option choice.  Using first choice as default.");
                }
                attKeys.Add(defaultChoice.Key);
            }

            return product.GetProductVariantDisplayWithAttributes(attKeys.ToArray());
        }


        /// <summary>
        /// Gets the <see cref="ProductVariantDisplay"/> with matching with attributes from the product.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="optionChoices">
        /// The option choices.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>        
        public static IProductVariantContent GetProductVariantDisplayWithAttributes(this IProductContent product, Guid[] optionChoices)
        {
            var variant = 
                product.ProductVariants.FirstOrDefault(
                    x =>
                    x.Attributes.Count() == optionChoices.Count()
                    && optionChoices.All(key => x.Attributes.FirstOrDefault(att => att.Key == key) != null));

            if (variant == null)
            {
                MultiLogHelper.Debug(typeof(ProductContentExtensions), "Could not find IProductVariantContent with keys matching choices in optionChoices array.");
            }

            return variant;
        }
    }
}