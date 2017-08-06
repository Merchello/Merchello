namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class Invoice2EntityCollectionDto
    {
        public Guid InvoiceKey { get; set; }

        public Guid EntityCollectionKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual EntityCollectionDto EntityCollectionDtoKeyNavigation { get; set; }

        public virtual InvoiceDto InvoiceDtoKeyNavigation { get; set; }
    }
}