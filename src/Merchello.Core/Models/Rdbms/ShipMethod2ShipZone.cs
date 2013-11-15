using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchShipMethod2ShipZone")]
    [PrimaryKey("shipMethodKey", autoIncrement = false)]
    [ExplicitColumns]
    public class ShipMethod2ShipZone
    {
        [Column("shipMethodKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchShipMethod2ShipZone", OnColumns = "shipMethodKey, shipZoneKey")]
        [ForeignKey(typeof(ShipMethodDto), Name = "FK_merchShipMethod2ShipZone_merchShipMethod", Column = "pk")]
        public Guid ShipMethodKey { get; set; }

        [Column("shipZoneKey")]
        [ForeignKey(typeof(ShipZoneDto), Name = "FK_merchShipMethod2ShipZone_merchShipZone", Column = "pk")]
        public Guid ShipZoneKey { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}