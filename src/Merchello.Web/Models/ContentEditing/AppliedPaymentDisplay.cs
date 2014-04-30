using System;

namespace Merchello.Web.Models.ContentEditing
{    
    public class AppliedPaymentDisplay
    {
        public Guid Key { get; set; }
        public Guid PaymentKey { get; set; }
        public Guid InvoiceKey { get; set; }
        public Guid AppliedPaymentTfKey { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public bool Exported { get; set; }
    }
}