namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The product 2 product collection dto.
    /// </summary>
    [TableName("merchProduct2ProductCollection")]
    [PrimaryKey("productKey", autoIncrement = false)]
    [ExplicitColumns]
    internal class Product2ProductCollectionDto
    {
        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        [Column("productKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProduct2ProductCollection", OnColumns = "productKey, productCollectionKey")]
        [ForeignKey(typeof(ProductDto), Name = "FK_merchProduct2ProductCollection_merchProduct", Column = "pk")]
        public Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the product collection key.
        /// </summary>
        [Column("productCollectionKey")]
        [ForeignKey(typeof(ProductCollectionDto), Name = "FK_merchProduct2ProductCollection_merchProductCollection", Column = "pk")]
        public Guid ProductCollectionKey { get; set; }

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
        /// Gets or sets the product collection.
        /// </summary>
        [ResultColumn]
        public ProductCollectionDto ProductCollection { get; set; }
    }
}