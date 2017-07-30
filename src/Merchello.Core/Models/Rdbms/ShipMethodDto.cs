namespace Merchello.Core.Models.Rdbms
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchShipMethod" table.
    /// </summary>
    internal class ShipMethodDto : IEntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ship country key.
        /// </summary>
        public Guid ShipCountryKey { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        public Guid ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the surcharge.
        /// </summary>
        public decimal Surcharge { get; set; }

        /// <summary>
        /// Gets or sets the service code.
        /// </summary>
        [CanBeNull]
        public string ServiceCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ship method is taxable.
        /// </summary>
        public bool Taxable { get; set; }

        /// <summary>
        /// Gets or sets the province data.
        /// </summary>
        [CanBeNull]
        public string ProvinceData { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}