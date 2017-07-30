namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchProductVariant2ProductAttribute" table.
    /// </summary>
    internal class ProductVariant2ProductAttributeDto : IDto
    {
        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        public Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the option key.
        /// </summary>
        public Guid OptionKey { get; set; }

        /// <summary>
        /// Gets or sets the product attribute key.
        /// </summary>
        public Guid ProductAttributeKey { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
