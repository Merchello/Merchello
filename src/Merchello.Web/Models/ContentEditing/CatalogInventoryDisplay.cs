namespace Merchello.Web.Models.ContentEditing
{
    using System;

    /// <summary>
    /// The catalog inventory display.
    /// </summary>
    public class CatalogInventoryDisplay
    {
        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        public Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the catalog key.
        /// </summary>
        public Guid CatalogKey { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the low count.
        /// </summary>
        public int LowCount { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }
}