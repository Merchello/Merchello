using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public class OrderDisplay
    {
        public Guid Key { get; set; }
        public Guid VersionKey { get; set; }
        public Guid InvoiceKey { get; set; }
        public string OrderNumberPrefix { get; set; }
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid OrderStatusKey { get; set; }
        public OrderStatusDisplay OrderStatus { get; set; }
        public bool Exported { get; set; }
        public IEnumerable<OrderLineItemDisplay> Items { get; set; }
    }
}