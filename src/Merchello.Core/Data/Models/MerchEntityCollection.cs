namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchEntityCollection
    {
        public MerchEntityCollection()
        {
            this.MerchCustomer2EntityCollection = new HashSet<MerchCustomer2EntityCollection>();
            this.MerchInvoice2EntityCollection = new HashSet<MerchInvoice2EntityCollection>();
            this.MerchProduct2EntityCollection = new HashSet<MerchProduct2EntityCollection>();
        }

        public Guid Pk { get; set; }

        public Guid? ParentKey { get; set; }

        public Guid EntityTfKey { get; set; }

        public string Name { get; set; }

        public int SortOrder { get; set; }

        public Guid ProviderKey { get; set; }

        public bool IsFilter { get; set; }

        public string ExtendedData { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<MerchCustomer2EntityCollection> MerchCustomer2EntityCollection { get; set; }

        public ICollection<MerchInvoice2EntityCollection> MerchInvoice2EntityCollection { get; set; }

        public ICollection<MerchProduct2EntityCollection> MerchProduct2EntityCollection { get; set; }

        public MerchEntityCollection ParentKeyNavigation { get; set; }

        public ICollection<MerchEntityCollection> InverseParentKeyNavigation { get; set; }
    }
}