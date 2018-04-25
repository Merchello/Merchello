using System;
using System.Collections.Generic;
using System.Linq;

namespace Merchello.Core.Models
{
    /// <summary>
    /// This is just a helper class to deal with invoice adjustments
    /// </summary>
    /// <remarks>
    /// Invoice adjustments are tricky because or orders, and shipments being tied to orders
    /// This class is just to help make this process a bit easier
    /// </remarks>
    public class InvoiceOrderShipment
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InvoiceOrderShipment()
        {
            Orders = new OrderCollection();
            LineItems = new List<InvoiceOrderShipmentLineItem>();
        }

        /// <summary>
        /// The invoice id
        /// </summary>
        public IInvoice Invoice { get; set; }

        /// <summary>
        /// The order Id
        /// </summary>
        public OrderCollection Orders { get; set; }

        /// <summary>
        /// Does this invoice have an order
        /// </summary>
        public bool HasOrders
        {
            get { return Orders.Any(); }
        }

        /// <summary>
        /// Line items
        /// </summary>
        public List<InvoiceOrderShipmentLineItem> LineItems { get; set; }
    }

    /// <summary>
    /// A helper class to tie together the Invoice and Order lineitems
    /// </summary>
    public class InvoiceOrderShipmentLineItem
    {
        /// <summary>
        /// The invoice line item id
        /// </summary>
        public Guid InvoiceLineItemId { get; set; }

        /// <summary>
        /// The order line item id
        /// </summary>
        public Guid? OrderLineItemId { get; set; }

        /// <summary>
        /// The order id
        /// </summary>
        public Guid? OrderId { get; set; }

        /// <summary>
        ///  The shipment id
        /// </summary>
        public Guid? ShipmentId { get; set; }

        /// <summary>
        /// Has a shipment been created
        /// </summary>
        public bool HasShipment
        {
            get { return ShipmentId != null; }
        }

        /// <summary>
        /// Is there an associated order line item
        /// </summary>
        public bool HasAssociatedOrderLineItem
        {
            get { return OrderLineItemId != null; }
        }

        /// <summary>
        /// Can this line item be updated
        /// </summary>
        public bool CanUpdate
        {
            get
            {
                if (HasShipment)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// The Product or Product variant id
        /// </summary>
        public Guid? ItemId { get; set; }

        /// <summary>
        /// The item SKU
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// The Item Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Item Price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The item Qty
        /// </summary>
        public int Qty { get; set; }
    }
}