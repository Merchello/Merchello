namespace Merchello.Web.Models.ContentEditing
{
    using System;

    /// <summary>
    /// The product attribute display.
    /// </summary>
    public class ProductAttributeDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the option key.
        /// </summary>
        public Guid OptionKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public int SortOrder { get; set; }
    }
}
