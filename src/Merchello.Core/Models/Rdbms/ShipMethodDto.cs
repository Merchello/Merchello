namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchShipMethod" table.
    /// </summary>
    [TableName("merchShipMethod")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class ShipMethodDto : EntityDto
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ship country key.
        /// </summary>
        [Column("shipCountryKey")]
        [ForeignKey(typeof(ShipCountryDto), Name = "FK_merchShipMethod_merchShipCountry", Column = "pk")]
        public Guid ShipCountryKey { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        [Column("providerKey")]
        [ForeignKey(typeof(GatewayProviderSettingsDto), Name = "FK_merchShipMethod_merchGatewayProviderSettings", Column = "pk")]
        public Guid ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the surcharge.
        /// </summary>
        [Column("surcharge")]
        [Constraint(Default = "0")]
        public decimal Surcharge { get; set; }

        /// <summary>
        /// Gets or sets the service code.
        /// </summary>
        [Column("serviceCode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ServiceCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ship method is taxable.
        /// </summary>
        [Column("taxable")]
        public bool Taxable { get; set; }

        /// <summary>
        /// Gets or sets the province data.
        /// </summary>
        [Column("provinceData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ProvinceData { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="GatewayProviderSettingsDto"/>.
        /// </summary>
        [ResultColumn]
        public GatewayProviderSettingsDto GatewayProviderSettingsDto { get; set; }
    }
}