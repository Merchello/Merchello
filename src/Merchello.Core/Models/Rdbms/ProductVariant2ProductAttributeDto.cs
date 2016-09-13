namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchProductVariant2ProductAttribute" table.
    /// </summary>
    [TableName("merchProductVariant2ProductAttribute")]
    [PrimaryKey("productVariantKey", AutoIncrement = false)]
    [ExplicitColumns]
    internal class ProductVariant2ProductAttributeDto : IDto
    {
        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        [Column("productVariantKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProductVariant2ProductAttribute", OnColumns = "productVariantKey, optionKey")]
        [ForeignKey(typeof(ProductVariantDto), Name = "FK_merchProductVariant2ProductAttribute_merchProductVariant", Column = "pk")]
        public Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the option key.
        /// </summary>
        [Column("optionKey")]
        [ForeignKey(typeof(ProductOptionDto), Name = "FK_merchProductVariant2ProductAttribute_merchProductOption", Column = "pk")]
        public Guid OptionKey { get; set; }

        /// <summary>
        /// Gets or sets the product attribute key.
        /// </summary>
        [Column("productAttributeKey")]
        [ForeignKey(typeof(ProductAttributeDto), Name = "FK_merchProductVariant2ProductAttribute_merchProductAttribute", Column = "pk")]
        public Guid ProductAttributeKey { get; set; }

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
        /// Gets or sets the product attribute DTO.
        /// </summary>
        [ResultColumn]
        public ProductAttributeDto ProductAttributeDto { get; set; }
    }
}
