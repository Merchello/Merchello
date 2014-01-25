using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchTaxMethod")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    public class TaxMethodDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("shipCountryKey")]
        [ForeignKey(typeof(ShipCountryDto), Name = "FK_merchTaxMethod_merchShipCountry", Column = "pk")]
        public Guid ShipCountryKey { get; set; }

        [Column("providerKey")]
        [ForeignKey(typeof(GatewayProviderDto), Name = "FK_merchTaxMethod_merchGatewayProvider", Column = "pk")]
        public Guid ProviderKey { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("rate")]
        [Constraint(Default = "0")]
        public decimal Rate { get; set; }        

        [Column("provinceData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ProvinceData { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; } 
    }
}