namespace Merchello.Web.Models.ContentEditing.Content
{
    using Umbraco.Web.Models.ContentEditing;

    /// <summary>
    /// Saves a product based display item's detached content.
    /// </summary>
    /// <typeparam name="TDisplay">
    /// The type of display product
    /// </typeparam>
    public abstract class ProductContentSaveItemBase<TDisplay> : DetachedContentSaveItem<TDisplay>
        where TDisplay : ProductDisplayBase
    {
        /// <summary>
        /// Gets or sets the culture name.
        /// </summary>
        public string CultureName { get; set; }
    }
}