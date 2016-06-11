namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The product variant index dto.
    /// </summary>
    [TableName("merchProductVariantIndex")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class ProductVariantIndexDto
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        [Column("productVariantKey")]
        [ForeignKey(typeof(ProductVariantDto), Name = "FK_merchProductVariantIndex_merchProductVariant", Column = "pk")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_merchProductVariantIndex")]
        public Guid ProductVariantKey { get; set; }

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
    }
}