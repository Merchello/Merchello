namespace Merchello.Core.Data.Models
{
    using System;

    // TODO drop this class
    // FYI we are not going to use Examine for invoices
    internal partial class MerchInvoiceIndex
    {
        public int Id { get; set; }

        public Guid InvoiceKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchInvoice InvoiceKeyNavigation { get; set; }
    }
}