namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchItemCache
    {
        public MerchItemCache()
        {
            this.MerchItemCacheItem = new HashSet<MerchItemCacheItem>();
        }

        public Guid Pk { get; set; }

        public Guid EntityKey { get; set; }

        public Guid ItemCacheTfKey { get; set; }

        public Guid VersionKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<MerchItemCacheItem> MerchItemCacheItem { get; set; }
    }
}