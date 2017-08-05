namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class MerchCustomer2EntityCollection
    {
        public Guid CustomerKey { get; set; }

        public Guid EntityCollectionKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchCustomer CustomerKeyNavigation { get; set; }

        public virtual MerchEntityCollection EntityCollectionKeyNavigation { get; set; }
    }
}