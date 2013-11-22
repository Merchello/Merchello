using System;

namespace Merchello.Web.Models.ContentEditing
{
    public class WarehouseInventoryDisplay
    {
        public Guid CatalogKey { get; set; }
        public Guid ProductVariantKey { get; set; }
        public Guid WarehouseKey { get; set; }
        public int Count { get; set; }
        public int LowCount { get; set; }
    }
}