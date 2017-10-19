namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class CustomerDto
    {
        public CustomerDto()
        {
            this.MerchCustomer2EntityCollection = new HashSet<Customer2EntityCollectionDto>();
            this.MerchCustomerAddress = new HashSet<CustomerAddressDto>();
            this.MerchInvoice = new HashSet<InvoiceDto>();
            this.MerchPayment = new HashSet<PaymentDto>();
        }

        public Guid Pk { get; set; }

        public string LoginName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool TaxExempt { get; set; }

        public DateTime LastActivityDate { get; set; }

        public string ExtendedData { get; set; }

        public string Notes { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<Customer2EntityCollectionDto> MerchCustomer2EntityCollection { get; set; }

        public ICollection<CustomerAddressDto> MerchCustomerAddress { get; set; }

        public CustomerIndexDto CustomerIndexDto { get; set; }

        public ICollection<InvoiceDto> MerchInvoice { get; set; }

        public ICollection<PaymentDto> MerchPayment { get; set; }
    }
}