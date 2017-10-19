namespace Merchello.Core.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    // TODO drop this class
    // FYI we are not going to use Examine for invoices
    internal class InvoiceIndexDto
    {
        public int Id { get; set; }

        [ForeignKey("InvoiceDto")]
        public Guid InvoiceKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        //public virtual InvoiceDto InvoiceDtoKeyNavigation { get; set; }
    }
}