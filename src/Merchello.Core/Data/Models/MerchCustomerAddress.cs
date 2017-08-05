namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class MerchCustomerAddress
    {
        public Guid Pk { get; set; }

        public Guid CustomerKey { get; set; }

        public string Label { get; set; }

        public string FullName { get; set; }

        public string Company { get; set; }

        public Guid AddressTfKey { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Locality { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public string CountryCode { get; set; }

        public string Phone { get; set; }

        public bool IsDefault { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchCustomer CustomerKeyNavigation { get; set; }
    }
}