namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;
    using Merchello.Core.Models.EntityBase;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchProductVariantDetachedContent" table.
    /// </summary>
    [TableName("merchProductVariantDetachedContent")]
    [PrimaryKey("productVariantKey", AutoIncrement = false)]
    [ExplicitColumns]
    internal class ProductVariantDetachedContentDto : EntityDto
    {
        /// <summary>
        /// Gets or sets the unique key.
        /// </summary>
        /// <remarks>
        /// This is weird situation where we want a Key so that we can use <see cref="IEntity"/> but the actual primary constraint needs to be 
        /// multiple keys.
        /// </remarks>
        [Column("pk")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_merchProductVariantDetachedContentKey")]
        public new Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        [Column("productVariantKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProductVariantDetachedContent", OnColumns = "productVariantKey, cultureName")]
        [ForeignKey(typeof(ProductVariantDto), Name = "FK_merchProductVariantDetachedContent_merchProductVariant", Column = "pk")]
        public Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the culture name.
        /// </summary>
        [Column("cultureName")]
        [Index(IndexTypes.NonClustered, Name = "IX_merchProductVariantDetachedContentCultureName")]
        public string CultureName { get; set; }

        /// <summary>
        /// Gets or sets the detached content type key.
        /// </summary>
        [Column("detachedContentTypeKey")]
        [ForeignKey(typeof(DetachedContentTypeDto), Name = "FK_merchProductVariantDetachedContent_merchDetachedContentTypeKey", Column = "pk")]
        public Guid DetachedContentTypeKey { get; set; }

        /// <summary>
        /// Gets or sets the property values.
        /// </summary>
        /// <remarks>
        /// Introduced in version 1.12.0
        /// Used to store property value JSON similar to the Nested Content package
        /// </remarks>
        [Column("values")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string Values { get; set; }

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
        /// Gets or sets a value indicating whether virtual content can be rendered.
        /// </summary>
        [Column("canBeRendered")]
        [Constraint(Default = "1")]
        public bool CanBeRendered { get; set; }

        /// <summary>
        /// Gets or sets the detached published content type.
        /// </summary>
        [ResultColumn]
        public DetachedContentTypeDto DetachedContentType { get; set; }
    }
}