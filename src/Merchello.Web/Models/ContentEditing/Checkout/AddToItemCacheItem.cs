namespace Merchello.Web.Models.ContentEditing.Checkout
{
    using System;

    /// <summary>
    /// The add to item cache item.
    /// </summary>
    public class AddToItemCacheItem
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is variant.
        /// </summary>
        public bool IsProductVariant { get; set; }
    }
}