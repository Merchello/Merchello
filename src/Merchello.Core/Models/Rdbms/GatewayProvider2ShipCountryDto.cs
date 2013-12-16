using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchGatewayProvider2ShipCountry")]
    [PrimaryKey("gatewayProviderKey", autoIncrement = false)]
    [ExplicitColumns]
    public class GatewayProvider2ShipCountryDto
    {
        [Column("gatewayProviderKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchGatewayProvider2ShipCountry", OnColumns = "gatewayProviderKey, shipCountryKey")]
        [ForeignKey(typeof(GatewayProviderDto), Name = "FK_merchGatewayProvider2ShipCountry_merchGatewayProvider", Column = "pk")]
        public Guid GatewayProviderKey { get; set; }

        [Column("shipCountryKey")]
        [ForeignKey(typeof(ShipCountryDto), Name = "FK_merchGatewayProvider2ShipCountry_merchShipCountry", Column = "pk")]
        public Guid OptionKey { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}