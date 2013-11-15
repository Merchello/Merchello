using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchShipZone")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    public class ShipZoneDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public int Key { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("countryRule")]
        public string CountryRule { get; set; }

        [Column("regionRule")]
        public string RegionRule { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}