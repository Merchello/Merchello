using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchShipMethod2Warehouse")]
    [PrimaryKey("shipMethodKey", autoIncrement = false)]
    [ExplicitColumns]
    internal class ShipMethod2WarehouseDto
    {
        [Column("shipMethodKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchShipMethod2Warehouse", OnColumns = "shipMethodKey, warehouseKey")]
        [ForeignKey(typeof(ShipMethodDto), Name = "FK_merchShipMethod2Warehouse_merchShipMethod", Column = "pk")]
        public Guid ShipMethodKey { get; set; }
        
        [Column("warehouseKey")]
        //[ForeignKey(typeof(WarehouseDto), Name = "FK_merchShipMethod2Warehouse_merchWarehouse", Column = "pk")]
        public Guid WarehouseKey { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}