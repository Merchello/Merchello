namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class MerchInvoice2EntityCollection
    {
        public Guid InvoiceKey { get; set; }

        public Guid EntityCollectionKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchEntityCollection EntityCollectionKeyNavigation { get; set; }

        public virtual MerchInvoice InvoiceKeyNavigation { get; set; }
    }
}