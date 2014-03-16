using System;
using System.Collections.Generic;

namespace Merchello.Web.Models.ContentEditing
{
    public class InvoiceDisplay
    {
        public Guid Key { get; set; }
        public Guid VersionKey { get; set; }       
        public Guid? CustomerKey { get; set; }
        public string InvoiceNumberPrefix { get; set; }
        public int InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public Guid InvoiceStatusKey { get; set; }
        public InvoiceStatusDisplay InvoiceStatus { get; set; }
        public string BillToName { get; set; }
        public string BillToAddress1 { get; set; }
        public string BillToAddress2 { get; set; }
        public string BillToLocality { get; set; }
        public string BillToRegion { get; set; }
        public string BillToPostalCode { get; set; }
        public string BillToCountryCode { get; set; }
        public string BillToEmail { get; set; }
        public string BillToPhone { get; set; }
        public string BillToCompany { get; set; }
        public bool Exported { get; set; }
        public bool Archived { get; set; }
        public IEnumerable<OrderDisplay> Orders { get; set; }
        public IEnumerable<InvoiceLineItemDisplay> Items { get; set; }
    }
}