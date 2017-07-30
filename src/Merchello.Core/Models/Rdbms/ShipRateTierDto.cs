namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchShipRateTier" table.
    /// </summary>
    internal class ShipRateTierDto : IEntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the ship method key.
        /// </summary>
        public Guid ShipMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the range low.
        /// </summary>
        public decimal RangeLow { get; set; }

        /// <summary>
        /// Gets or sets the range high.
        /// </summary>
        public decimal RangeHigh { get; set; }

        /// <summary>
        /// Gets or sets the rate.
        /// </summary>
        public decimal Rate { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}