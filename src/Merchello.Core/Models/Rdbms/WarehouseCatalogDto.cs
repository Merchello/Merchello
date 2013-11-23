using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchWarehouseCatalog")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class WarehouseCatalogDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("warehouseKey")]
        [ForeignKey(typeof(WarehouseDto), Name = "FK_merchWarehouseCatalog_merchWarehouse", Column = "pk")]
        public Guid WarehouseKey { get; set; }

        [Column("name")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Name { get; set; }

        [Column("description")]
        [Length(500)]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Description { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}