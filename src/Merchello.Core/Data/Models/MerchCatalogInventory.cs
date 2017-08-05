namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class MerchCatalogInventory
    {
        public Guid CatalogKey { get; set; }

        public Guid ProductVariantKey { get; set; }

        public int Count { get; set; }

        public int LowCount { get; set; }

        public string Location { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchWarehouseCatalog CatalogKeyNavigation { get; set; }

        public virtual MerchProductVariant ProductVariantKeyNavigation { get; set; }
    }
}