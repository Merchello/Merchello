namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The product 2 product collection dto.
    /// </summary>
    [TableName("merchProduct2EntityCollection")]
    [PrimaryKey("productKey", autoIncrement = false)]
    [ExplicitColumns]
    internal class Product2EntityCollectionDto
    {
        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        [Column("productKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProduct2EntityCollection", OnColumns = "productKey, entityCollectionKey")]
        [ForeignKey(typeof(ProductDto), Name = "FK_merchProduct2EnityCollection_merchProduct", Column = "pk")]
        public Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the product collection key.
        /// </summary>
        [Column("entityCollectionKey")]
        [ForeignKey(typeof(EntityCollectionDto), Name = "FK_merchProduct2EntityCollection_merchEntityCollection", Column = "pk")]
        public Guid EntityCollectionKey { get; set; }

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