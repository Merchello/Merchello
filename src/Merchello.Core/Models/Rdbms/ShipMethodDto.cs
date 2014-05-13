using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchShipMethod")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class ShipMethodDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("shipCountryKey")]
        [ForeignKey(typeof(ShipCountryDto), Name = "FK_merchShipMethod_merchShipCountry", Column = "pk")]
        public Guid ShipCountryKey { get; set; }

        [Column("providerKey")]
        [ForeignKey(typeof(GatewayProviderSettingsDto), Name = "FK_merchShipMethod_merchGatewayProviderSettings", Column = "pk")]
        public Guid ProviderKey { get; set; }

        [Column("surcharge")]
        [Constraint(Default = "0")]
        public decimal Surcharge { get; set; }        

        [Column("serviceCode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ServiceCode { get; set; }

        [Column("taxable")]
        public bool Taxable { get; set; }

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

        [ResultColumn]
        public GatewayProviderSettingsDto GatewayProviderSettingsDto { get; set; }

    }
}