namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// A DTO object for performing product attribute related data operations.
    /// </summary>
    [TableName("merchProductAttribute")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class ProductAttributeDto : EntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("pk")]
        [Constraint(Default = "newid()")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the option key.
        /// </summary>
        [Column("optionKey")]
        [ForeignKey(typeof(ProductOptionDto), Name = "FK_merchProductAttribute_merchOption", Column = "pk")]
        public Guid OptionKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        [Column("sku")]
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the detached content values.
        /// </summary>
        [Column("detachedContentValues")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string DetachedContentValues { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        [Column("sortOrder")]
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is default choice.
        /// </summary>
        [Column("isDefaultChoice")]
        [Constraint(Default = "0")]
        public bool IsDefaultChoice { get; set; }
    }
}
