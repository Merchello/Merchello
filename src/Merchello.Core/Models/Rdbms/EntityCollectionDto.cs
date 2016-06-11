namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The product collection dto.
    /// </summary>
    [TableName("merchEntityCollection")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class EntityCollectionDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

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