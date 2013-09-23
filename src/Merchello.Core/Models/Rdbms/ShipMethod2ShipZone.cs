using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchShipMethod2ShipZone")]
    [PrimaryKey("shipMethodId", autoIncrement = false)]
    [ExplicitColumns]
    public class ShipMethod2ShipZone
    {
        [Column("shipMethodId")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchShipMethod2ShipZone", OnColumns = "shipMethodId, shipZoneId")]
        [ForeignKey(typeof(ShipMethodDto), Name = "FK_merchShipMethod2ShipZone_merchShipMethod", Column = "id")]
        public int ShipMethodId { get; set; }

        [Column("shipZoneId")]
        [ForeignKey(typeof(ShipZoneDto), Name = "FK_merchShipMethod2ShipZone_merchShipZone", Column = "id")]
        public int ShipZoneId { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}