using System;
using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public class InvoiceLineItemDisplay
    {
        public Guid Key { get; set; }
        public Guid ContainerKey { get; set; }
        public Guid LineItemTfKey { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool Exported { get; set; }
    }
}