using System;
using System.Collections.Generic;

namespace Merchello.Web.Models.ContentEditing
{
    public class OrderLineItemDisplay
    {
        public Guid Key { get; set; }
        public Guid ContainerKey { get; set; }
        public Guid? ShipmentKey { get; set; }
        public Guid LineItemTfKey { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool Exported { get; set; }
        public bool BackOrder { get; set; }
        public IEnumerable<KeyValuePair<string, string>> ExtendedData { get; set; }
    }
}