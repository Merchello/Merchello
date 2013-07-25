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

        // TODO: RSS - confirm with Joe: This was a char(10) -> should'nt it be an int
        [Column("shipMethodTypeId")]
        public string ShipMethodTypeId { get; set; }

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