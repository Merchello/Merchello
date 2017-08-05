namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class MerchItemCacheItem
    {
        public Guid Pk { get; set; }

        public Guid ItemCacheKey { get; set; }

        public Guid LineItemTfKey { get; set; }

        public string Sku { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string ExtendedData { get; set; }

        public bool Exported { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchItemCache ItemCacheKeyNavigation { get; set; }
    }
}