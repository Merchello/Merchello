namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The ship fixed rate table display.
    /// </summary>
    public class ShipFixedRateTableDisplay
    {
        /// <summary>
        /// Gets or sets the ship method key.
        /// </summary>
        public Guid ShipMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        public IEnumerable<ShipRateTierDisplay> Rows { get; set; }
    }
}
