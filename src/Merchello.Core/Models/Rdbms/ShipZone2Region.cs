using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchShipZone2Region")]
    [PrimaryKey("shipZoneKey", autoIncrement = false)]
    [ExplicitColumns]
    public class ShipZone2Region
    {
        [Column("shipZoneKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchShipZone2Region", OnColumns = "shipZoneId, regionCode")]
        [ForeignKey(typeof(ShipZoneDto), Name = "FK_merchShipZone2Region_merchShipZone", Column = "pk")]
        public Guid ShipZoneKey { get; set; }

        [Column("regionCode")]
        [Length(2)]
        [ForeignKey(typeof(RegionDto), Name = "FK_merchShipZone2Region_merchRegion", Column = "regionCode")]
        public string RegionCode { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}