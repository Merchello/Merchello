namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The merchTaxMethod table.
    /// </summary>
    [TableName("merchTaxMethod")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class TaxMethodDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        [Column("providerKey")]
        [ForeignKey(typeof(GatewayProviderSettingsDto), Name = "FK_merchTaxMethod_merchGatewayProviderSettings", Column = "pk")]
        public Guid ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        [Column("countryCode")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the percentage tax rate.
        /// </summary>
        [Column("percentageTaxRate")]
        [Constraint(Default = "0")]
        public decimal PercentageTaxRate { get; set; }

        /// <summary>
        /// Gets or sets the province data.
        /// </summary>
        [Column("provinceData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ProvinceData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product tax method.
        /// </summary>
        [Column("productTaxMethod")]
        public bool ProductTaxMethod { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; } 
    }
}