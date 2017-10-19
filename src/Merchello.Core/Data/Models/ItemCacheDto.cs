namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed class ItemCacheDto
    {
        public ItemCacheDto()
        {
            this.MerchItemCacheItem = new HashSet<ItemCacheItemDto>();
        }

        public Guid Pk { get; set; }

        public Guid EntityKey { get; set; }

        public Guid ItemCacheTfKey { get; set; }

        public Guid VersionKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<ItemCacheItemDto> MerchItemCacheItem { get; set; }
    }
}