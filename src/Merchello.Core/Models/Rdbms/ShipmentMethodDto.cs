using System;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchShipMethod")]
    [PrimaryKey("id", autoIncrement = false)]
    [ExplicitColumns]
    public class ShipMethodDto
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("gatewayAlias")]
        public int GatewayAlias { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("surcharge")]
        public decimal Surcharge { get; set; }

        [Column("serviceCode")]
        public string ServiceCode { get; set; }

        [Column("updateDate")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

    }
}