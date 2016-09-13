namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchProductOptionAttributeShare" table.
    /// </summary>
    [TableName("merchProductOptionAttributeShare")]
    [PrimaryKey("optionKey", AutoIncrement = false)]
    [ExplicitColumns]
    internal class ProductOptionAttributeShareDto : IDto
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
        /// Gets or sets a value indicating whether is default choice.
        /// </summary>
        [Column("isDefaultChoice")]
        public bool IsDefaultChoice { get; set; }

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