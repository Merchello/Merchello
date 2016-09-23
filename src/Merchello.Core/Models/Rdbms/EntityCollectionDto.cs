namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The product collection dto.
    /// </summary>
    [TableName("merchEntityCollection")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class EntityCollectionDto : EntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("pk")]
        [Constraint(Default = "newid()")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the parent key.
        /// </summary>
        [Column("parentKey")]
        [ForeignKey(typeof(EntityCollectionDto), Name = "FK_merchEntityCollection_merchEntityCollection", Column = "pk")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid? ParentKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        [Column("entityTfKey")]
        public Guid EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        [Column("sortOrder")]
        [Constraint(Default = "0")]
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the key for the collection provider.
        /// </summary>
        [Column("providerKey")]
        public Guid ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is filter.
        /// </summary>
        [Column("isFilter")]
        public bool IsFilter { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        [Column("extendedData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ExtendedData { get; set; }
    }
}