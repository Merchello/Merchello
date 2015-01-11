namespace Merchello.Web.Models.Shipping
{
    using System.Collections.Generic;

    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// API wrapper to return a <see cref="ShipMethodDisplay"/> and alternative <see cref="ShipMethodDisplay"/>.
    /// </summary>
    public class ShipMethodsQueryDisplay
    {
        /// <summary>
        /// Gets or sets the currently selected <see cref="ShipMethodDisplay"/>.
        /// </summary>
        public ShipMethodDisplay Selected { get; set; }

        /// <summary>
        /// Gets or sets the set of alternative <see cref="ShipMethodDisplay"/>s.
        /// </summary>
        public IEnumerable<ShipMethodDisplay> Alternatives { get; set; }
    }
}