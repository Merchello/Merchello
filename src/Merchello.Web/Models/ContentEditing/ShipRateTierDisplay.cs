namespace Merchello.Web.Models.ContentEditing
{
    using System;

    /// <summary>
    /// The ship rate tier display.
    /// </summary>
    public class ShipRateTierDisplay
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
    }
}
