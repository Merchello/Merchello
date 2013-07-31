using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchShipMethod")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class ShipMethodDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("gatewayAlias")]
        public int GatewayAlias { get; set; }

        [Column("typeKey")]
        public Guid TypeKey { get; set; }

        [Column("surcharge")]
        public decimal Surcharge { get; set; }

        [Column("serviceCode")]
        public string ServiceCode { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}