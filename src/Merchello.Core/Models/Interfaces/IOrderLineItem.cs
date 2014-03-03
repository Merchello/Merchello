using System;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines an Order Line Item
    /// </summary>
    public interface IOrderLineItem : ILineItem
    {
        /// <summary>
        /// The unique key (guid) associated with the shipment record in which this item was shipped.
        /// </summary>
        Guid? ShipmentKey { get; set; }

        /// <summary>
        /// True false indicating whether or not this line item represents a back order line item
        /// </summary>
        bool BackOrder { get; set; }
    }
}