namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class AppliedPaymentDto
    {
        public Guid Pk { get; set; }

        public Guid PaymentKey { get; set; }

        public Guid InvoiceKey { get; set; }

        public Guid AppliedPaymentTfKey { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public bool Exported { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchInvoice InvoiceKeyNavigation { get; set; }

        public virtual MerchPayment PaymentKeyNavigation { get; set; }
    }
}