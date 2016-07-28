namespace Merchello.Web.Models.ContentEditing.Content
{
    /// <summary>
    /// A container for saving product attribute extended content.
    /// </summary>
    public class ProductAttributeContentSave : DetachedContentSaveItem<ProductAttributeDisplay>
    {
        /// <summary>
        /// Gets or sets the detached content type.
        /// </summary>
        public DetachedContentTypeDisplay DetachedContentType { get; set; }
    }
}