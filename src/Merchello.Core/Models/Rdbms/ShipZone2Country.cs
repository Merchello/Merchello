using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchShipZone2Country")]
    [PrimaryKey("shipZoneId", autoIncrement = false)]
    [ExplicitColumns]
    public class ShipZone2Country
    {
        [Column("shipZoneId")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchShipZone2Country", OnColumns = "shipZoneId, countryCode")]
        [ForeignKey(typeof(ShipZoneDto), Name = "FK_merchShipZone2Country_merchShipZone", Column = "id")]
        public int ShipZoneId { get; set; }

        [Column("countryCode")]
        [Length(2)]
        [ForeignKey(typeof(CountryDto), Name = "FK_merchShipZone2Country_merchCountry", Column = "countryCode")]
        public string CountryCode { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}