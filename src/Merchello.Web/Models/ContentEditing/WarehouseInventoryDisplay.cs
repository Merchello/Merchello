using System;

namespace Merchello.Web.Models.ContentEditing
{
    public class WarehouseInventoryDisplay
    {
        public Guid ProductVariantKey { get; set; }
        public Guid WarehouseCatalogKey { get; set; }
        public int Count { get; set; }
        public int LowCount { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}