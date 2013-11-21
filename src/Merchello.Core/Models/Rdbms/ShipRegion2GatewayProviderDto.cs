using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchShipRegion2GatewayProvider")]
    [PrimaryKey("shipRegionKey", autoIncrement = false)]
    [ExplicitColumns]
    public class ShipRegion2GatewayProviderDto
    {
        [Column("shipRegionKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchRegion2GatewayProvider", OnColumns = "shipRegionKey, providerKey")]
        [ForeignKey(typeof(ShipRegionDto), Name = "FK_merchRegion2GatewayProvider_merchShipRegion", Column = "pk")]
        public Guid ShipRegionKey { get; set; }

        [Column("providerKey")]
        [ForeignKey(typeof(GatewayProviderDto), Name = "FK_merchRegion2GatewayProvider_merchGatewayProvider", Column = "pk")]
        public Guid ProviderKey { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}