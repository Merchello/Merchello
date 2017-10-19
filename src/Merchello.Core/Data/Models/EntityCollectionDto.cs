namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class EntityCollectionDto
    {
        public EntityCollectionDto()
        {
            this.MerchCustomer2EntityCollection = new HashSet<Customer2EntityCollectionDto>();
            this.MerchInvoice2EntityCollection = new HashSet<Invoice2EntityCollectionDto>();
            this.MerchProduct2EntityCollection = new HashSet<Product2EntityCollectionDto>();
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

        public ICollection<Customer2EntityCollectionDto> MerchCustomer2EntityCollection { get; set; }

        public ICollection<Invoice2EntityCollectionDto> MerchInvoice2EntityCollection { get; set; }

        public ICollection<Product2EntityCollectionDto> MerchProduct2EntityCollection { get; set; }

        public EntityCollectionDto ParentKeyNavigation { get; set; }

        public ICollection<EntityCollectionDto> InverseParentKeyNavigation { get; set; }
    }
}