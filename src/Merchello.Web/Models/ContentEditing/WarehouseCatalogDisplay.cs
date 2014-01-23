using System;

namespace Merchello.Web.Models.ContentEditing
{
    public class WarehouseCatalogDisplay
    {
        public Guid Key { get; set; }
        public Guid WarehouseKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
