using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchCountryTaxRate")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class CountryTaxRateDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("providerKey")]
        [ForeignKey(typeof(GatewayProviderDto), Name = "FK_merchCountryTaxRate_merchGatewayProvider", Column = "pk")]
        public Guid ProviderKey { get; set; }

        [Column("countryCode")]
        public string CountryCode { get; set; }

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