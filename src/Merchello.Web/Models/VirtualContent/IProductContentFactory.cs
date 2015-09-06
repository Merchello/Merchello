namespace Merchello.Web.Models.VirtualContent
{
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// Defines a ProductContentFactory.
    /// </summary>
    internal interface IProductContentFactory
    {
        /// <summary>
        /// The build content.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="cultureName">
        /// The culture name
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        IProductContent BuildContent(ProductDisplay display, string cultureName);

        /// <summary>
        /// The build content.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="cultureName">
        /// The culture name.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariantContent"/>.
        /// </returns>
        IProductVariantContent BuildContent(ProductVariantDisplay display, string cultureName);
    }
}