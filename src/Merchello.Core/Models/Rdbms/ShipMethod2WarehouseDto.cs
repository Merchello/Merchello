using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchShipMethod2Warehouse")]
    [PrimaryKey("shipMethodId", autoIncrement = false)]
    [ExplicitColumns]
    internal class ShipMethod2WarehouseDto
    {
        [Column("shipMethodId")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchShipMethod2Warehouse", OnColumns = "shipMethodId, warehouseId")]
        [ForeignKey(typeof(ShipMethodDto), Name = "FK_merchShipMethod2Warehouse_merchShipMethod", Column = "id")]
        public int ShipMethodId { get; set; }

        
        [Column("warehouseId")]
        [ForeignKey(typeof(WarehouseDto), Name = "FK_merchShipMethod2Warehouse_merchWarehouse", Column = "id")]
        public int WarehouseId { get; set; }


        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}