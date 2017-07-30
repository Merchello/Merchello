using System;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents an order Line Item
    /// </summary>
    public interface IOrderLineItem : ILineItem
    {
        /// <summary>
        /// Gets or sets the key associated with the shipment record in which this item was shipped.
        /// </summary>
        Guid? ShipmentKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the line item represents a back order line item.
        /// </summary>
        bool BackOrder { get; set; }
    }
}