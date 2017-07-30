namespace Merchello.Core.Models.Rdbms
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchGatewayProviderSettings" table.
    /// </summary>
    internal class GatewayProviderSettingsDto : IEntityDto, IExtendedDataDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the provider type field key.
        /// </summary>
        public Guid ProviderTfKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [CanBeNull]
        public string Description { get; set; }

        /// <inheritdoc />
        [CanBeNull]
        public string ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether encrypt extended data serialization should be encrypted.
        /// </summary>
        /// <remarks>
        /// Encryption is based on the local machine key so this value must be false to move data between environments.
        /// </remarks>
        public bool EncryptExtendedData { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ShipMethodDto"/>.
        /// </summary>
        public List<ShipMethodDto> ShipMethods { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PaymentMethodDto"/>.
        /// </summary>
        public List<PaymentMethodDto> PaymentMethods { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TaxMethodDto"/>.
        /// </summary>
        public List<TaxMethodDto> TaxMethods { get; set; }
    }
}