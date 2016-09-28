namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchGatewayProviderSettings" table.
    /// </summary>
    [TableName("merchGatewayProviderSettings")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class GatewayProviderSettingsDto : EntityDto, IExtendedDataDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("pk")]
        [Constraint(Default = "newid()")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the provider type field key.
        /// </summary>
        [Column("providerTfKey")]
        public Guid ProviderTfKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Column("description")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Description { get; set; }

        /// <inheritdoc />
        [Column("extendedData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether encrypt extended data serialization should be encrypted.
        /// </summary>
        /// <remarks>
        /// Encryption is based on the local machine key so this value must be false to move data between environments.
        /// </remarks>
        [Column("encryptExtendedData")]
        [Constraint(Default = "0")]
        public bool EncryptExtendedData { get; set; }
    }
}