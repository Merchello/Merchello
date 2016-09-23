namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchProductOption" table.
    /// </summary>
    [TableName("merchProductOption")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class ProductOptionDto : EntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("pk")]
        [Constraint(Default = "newid()")]
        public override Guid Key { get; set; }

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
        public Guid? DetachedContentTypeKey { get; set; }

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
        /// Gets or sets the UI option.
        /// </summary>
        [Column("uiOption")]
        [Length(50)]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string UiOption { get; set; }

            /// <summary>
        /// Gets or sets the result for product to product option association.
        /// </summary>
        [ResultColumn]
        public Product2ProductOptionDto Product2ProductOptionDto { get; set; }
    }
}
