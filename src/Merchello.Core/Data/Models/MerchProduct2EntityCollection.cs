namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class MerchProduct2EntityCollection
    {
        public Guid ProductKey { get; set; }

        public Guid EntityCollectionKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchEntityCollection EntityCollectionKeyNavigation { get; set; }

        public virtual MerchProduct ProductKeyNavigation { get; set; }
    }
}