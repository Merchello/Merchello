namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The product 2 product option dto.
    /// </summary>
    [TableName("merchProductOptionAttributeShare")]
    [PrimaryKey("optionKey", autoIncrement = false)]
    [ExplicitColumns]
    internal class ProductOptionAttributeShareDto
    {
        /// <summary>
        /// Gets or sets the option key.
        /// </summary>
        [Column("productKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProductOptionAttributeShare", OnColumns = "productKey, optionKey, attributeKey")]
        [ForeignKey(typeof(ProductDto), Name = "FK_merchProductOptionAttributeShare_merchProduct", Column = "pk")]
        public Guid ProductKey { get; set; }


        /// <summary>
        /// Gets or sets the option key.
        /// </summary>
        [Column("optionKey")]
        [ForeignKey(typeof(ProductOptionDto), Name = "FK_merchProductOptionAttributeShare_merchProductOption", Column = "pk")]
        public Guid OptionKey { get; set; }

        /// <summary>
        /// Gets or sets the product attribute key.
        /// </summary>
        [Column("attributeKey")]
        [ForeignKey(typeof(ProductAttributeDto), Name = "FK_merchProductOptionAttributeShare_merchProductAttribute", Column = "pk")]
        public Guid AttributeKey { get; set; }

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