namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;
    using Umbraco.Web.UI.Umbraco.Masterpages;

    /// <summary>
    /// DTO object for a product option.
    /// </summary>
    [TableName("merchProductOption")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class ProductOptionDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the detached content type key.
        /// </summary>
        [Column("detachedContentTypeKey")]
        [ForeignKey(typeof(DetachedContentTypeDto), Name = "FK_merchProductOptionDetachedContent_merchProductOption", Column = "pk")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid DetachedContentTypeKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether required.
        /// </summary>
        [Column("required")]
        [Constraint(Default = "0")]
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this represents a shared option.
        /// </summary>
        [Column("shared")]
        [Constraint(Default = "0")]
        public bool Shared { get; set; }

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
        /// Gets or sets the detached content type.
        /// </summary>
        [ResultColumn]
        public DetachedContentTypeDto DetachedContentType { get; set; }

        /// <summary>
        /// Gets or sets the result for product to product option association.
        /// </summary>
        [ResultColumn]
        public Product2ProductOptionDto Product2ProductOptionDto { get; set; }
    }
}
