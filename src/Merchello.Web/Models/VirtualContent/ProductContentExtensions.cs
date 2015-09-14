namespace Merchello.Web.Models.VirtualContent
{
    using System.Globalization;

    using Merchello.Web.Models.ContentEditing;

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
    }
}