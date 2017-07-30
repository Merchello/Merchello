namespace Merchello.Core.Models.Rdbms
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchTaxMethod" table.
    /// </summary>
    internal class TaxMethodDto : IEntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        public Guid ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [CanBeNull]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the percentage tax rate.
        /// </summary>
        public decimal PercentageTaxRate { get; set; }

        /// <summary>
        /// Gets or sets the province data.
        /// </summary>
        [CanBeNull]
        public string ProvinceData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product tax method.
        /// </summary>
        public bool ProductTaxMethod { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}