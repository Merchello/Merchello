namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The order display.
    /// </summary>
    public class OrderDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        public Guid VersionKey { get; set; }

        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the order number prefix.
        /// </summary>
        public string OrderNumberPrefix { get; set; }

        /// <summary>
        /// Gets or sets the order number.
        /// </summary>
        public int OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the order date.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Gets or sets the order status key.
        /// </summary>
        public Guid OrderStatusKey { get; set; }

        /// <summary>
        /// Gets or sets the order status.
        /// </summary>
        public OrderStatusDisplay OrderStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether exported.
        /// </summary>
        public bool Exported { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public IEnumerable<OrderLineItemDisplay> Items { get; set; }
    }
}