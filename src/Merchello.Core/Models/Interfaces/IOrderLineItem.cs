namespace Merchello.Core.Models
{
    using System;

    /// <summary>
    /// Represents anOrder Line Item
    /// </summary>
    public interface IOrderLineItem : ILineItem
    {
        /// <summary>
        /// Gets or sets the unique key associated with the shipment record in which this item was shipped.
        /// </summary>
        Guid? ShipmentKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this line item represents a back order line item
        /// </summary>
        bool BackOrder { get; set; }
    }
}