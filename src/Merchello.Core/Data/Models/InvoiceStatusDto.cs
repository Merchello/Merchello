namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed class InvoiceStatusDto
    {
        public InvoiceStatusDto()
        {
            this.MerchInvoice = new HashSet<InvoiceDto>();
        }

        public Guid Pk { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public bool Reportable { get; set; }

        public bool Active { get; set; }

        public int SortOrder { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<InvoiceDto> MerchInvoice { get; set; }
    }
}