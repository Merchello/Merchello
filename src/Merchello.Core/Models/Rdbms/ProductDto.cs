namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchProduct" table.
    /// </summary>
    [TableName("merchProduct")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class ProductDto : EntityDto, IPageableDto
    {
        /// <summary>
        /// Gets or sets the product variant dto.
        /// </summary>
        [ResultColumn]
        public ProductVariantDto ProductVariantDto { get; set; }
    }
}
