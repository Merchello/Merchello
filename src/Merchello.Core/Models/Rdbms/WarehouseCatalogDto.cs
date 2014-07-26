namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The warehouse catalog dto.
    /// </summary>
    [TableName("merchWarehouseCatalog")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class WarehouseCatalogDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the warehouse key.
        /// </summary>
        [Column("warehouseKey")]
        [ForeignKey(typeof(WarehouseDto), Name = "FK_merchWarehouseCatalog_merchWarehouse", Column = "pk")]
        public Guid WarehouseKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Column("description")]
        [Length(500)]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Description { get; set; }

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