namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchWarehouseCatalog
    {
        public MerchWarehouseCatalog()
        {
            this.MerchCatalogInventory = new HashSet<CatalogInventoryDto>();
            this.MerchShipCountry = new HashSet<MerchShipCountry>();
        }

        public Guid Pk { get; set; }

        public Guid WarehouseKey { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<CatalogInventoryDto> MerchCatalogInventory { get; set; }

        public ICollection<MerchShipCountry> MerchShipCountry { get; set; }

        public MerchWarehouse WarehouseKeyNavigation { get; set; }
    }
}