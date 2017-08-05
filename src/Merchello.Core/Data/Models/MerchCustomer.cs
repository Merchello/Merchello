namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchCustomer
    {
        public MerchCustomer()
        {
            this.MerchCustomer2EntityCollection = new HashSet<MerchCustomer2EntityCollection>();
            this.MerchCustomerAddress = new HashSet<MerchCustomerAddress>();
            this.MerchInvoice = new HashSet<MerchInvoice>();
            this.MerchPayment = new HashSet<MerchPayment>();
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

        public ICollection<MerchCustomer2EntityCollection> MerchCustomer2EntityCollection { get; set; }

        public ICollection<MerchCustomerAddress> MerchCustomerAddress { get; set; }

        public MerchCustomerIndex MerchCustomerIndex { get; set; }

        public ICollection<MerchInvoice> MerchInvoice { get; set; }

        public ICollection<MerchPayment> MerchPayment { get; set; }
    }
}