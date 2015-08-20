namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The product 2 product option dto.
    /// </summary>
    [TableName("merchProduct2ProductOption")]
    [PrimaryKey("productKey", autoIncrement = false)]
    [ExplicitColumns]
    internal class Product2ProductOptionDto
    {
        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        [Column("productKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProduct2Option", OnColumns = "productKey, optionKey")]
        [ForeignKey(typeof(ProductDto), Name = "FK_merchProduct2Option_merchProduct", Column = "pk")]
        public Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the option key.
        /// </summary>
        [Column("optionKey")]
        [ForeignKey(typeof(ProductOptionDto), Name = "FK_merchProduct2Option_merchOption", Column = "pk")]
        public Guid OptionKey { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        [Column("sortOrder")]
        public int SortOrder { get; set; }

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
        /// Gets or sets the product option dto.
        /// </summary>
        [ResultColumn]
        public ProductOptionDto ProductOptionDto { get; set; }
    }
}
