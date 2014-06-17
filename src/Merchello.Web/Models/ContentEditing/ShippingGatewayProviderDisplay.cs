namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;
    using Core.Models;

    /// <summary>
    /// The shipping gateway provider display.
    /// </summary>
    public class ShippingGatewayProviderDisplay
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
        /// Gets or sets the extended data.
        /// </summary>
        public ExtendedDataCollection ExtendedData { get; set; } 

        /// <summary>
        /// Gets or sets the ship methods.
        /// </summary>
        public IEnumerable<ShipMethodDisplay> ShipMethods { get; set; }
    }
}
