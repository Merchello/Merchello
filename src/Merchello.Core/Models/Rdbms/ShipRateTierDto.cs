using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{

    [TableName("merchShipRateTier")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class ShipRateTierDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("shipMethodKey")]
        [ForeignKey(typeof(ShipMethodDto), Name = "FK_merchShipmentRateTier_merchShipMethod", Column = "pk")]
        public Guid ShipMethodKey { get; set; }

        [Column("rangeLow")]
        public decimal RangeLow { get; set; }

        [Column("rangeHigh")]
        public decimal RangeHigh { get; set; }

        [Column("rate")]
        public decimal Rate { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}