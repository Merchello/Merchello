namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchProduct" table.
    /// </summary>
    internal class ProductDto : IEntityDto, IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the product variant dto.
        /// </summary>
        public ProductVariantDto ProductVariantDto { get; set; }
    }
}
