namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The product variant 2 detached content dto.
    /// </summary>
    [TableName("merchProductVariantDetachedContent")]
    [PrimaryKey("productVariantKey", autoIncrement = false)]
    [ExplicitColumns]
    public class ProductVariantDetachedContentDto
    {
        /// <summary>
        /// Gets or sets the unique key.
        /// </summary>
        [Column("uniqueKey")]
        [IndexAttribute(IndexTypes.UniqueNonClustered, Name = "IX_merchProductVariantDetachedContentUniqueKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        [Column("productVariantKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProductVariantDetachedContent", OnColumns = "productVariantKey, detachedContentTypeKey,cultureName")]
        [ForeignKey(typeof(ProductVariantDto), Name = "FK_merchProductVariantDetachedContent_merchProductVariant", Column = "pk")]
        public Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the detached content type key.
        /// </summary>
        [Column("detachedContentTypeKey")]
        [ForeignKey(typeof(DetachedContentTypeDto), Name = "FK_merchProductVariantDetachedContent_merchDetachedContentTypeKey", Column = "pk")]
        public Guid DetachedContentTypeKey { get; set; }

        /// <summary>
        /// Gets or sets the culture name.
        /// </summary>
        [Column("cultureName")]
        [Index(IndexTypes.NonClustered, Name = "IX_merchProductVariantDetachedContentCultureName")]
        public string CultureName { get; set; }

        /// <summary>
        /// Gets or sets the detached content values.
        /// </summary>
        /// <remarks>
        /// Introduced in version 1.11.0
        /// Used to store property value JSON similar to the Nested Content package
        /// </remarks>
        [Column("detachedContentValues")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string DetachedContentValues { get; set; }

        /// <summary>
        /// Gets or sets the template id.
        /// </summary>
        [Column("templateId")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        [Column("slug")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Slug { get; set; }

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
        /// Gets or sets the detached published content type.
        /// </summary>
        [ResultColumn]
        public DetachedContentTypeDto DetachedContentType { get; set; }
    }
}