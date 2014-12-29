namespace Merchello.Core.Models.Rdbms
{
    using System;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The product dto.
    /// </summary>
    [TableName("merchProduct")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    public class ProductDto : IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the product variant dto.
        /// </summary>
        [ResultColumn]
        public ProductVariantDto ProductVariantDto { get; set; }

    }
}
