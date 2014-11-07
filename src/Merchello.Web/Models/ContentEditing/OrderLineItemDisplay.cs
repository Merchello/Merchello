namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The order line item display.
    /// </summary>
    public class OrderLineItemDisplay : LineItemDisplayBase
    {
        /// <summary>
        /// Gets or sets the shipment key.
        /// </summary>
        public Guid? ShipmentKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the line item represents a back ordered line item.
        /// </summary>
        public bool BackOrder { get; set; }
    }
}