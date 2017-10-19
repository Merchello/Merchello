namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed class WarehouseCatalogDto
    {
        public WarehouseCatalogDto()
        {
            this.MerchCatalogInventory = new HashSet<CatalogInventoryDto>();
            this.MerchShipCountry = new HashSet<ShipCountryDto>();
        }

        public Guid Pk { get; set; }

        public Guid WarehouseKey { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<CatalogInventoryDto> MerchCatalogInventory { get; set; }

        public ICollection<ShipCountryDto> MerchShipCountry { get; set; }

        public WarehouseDto WarehouseDtoKeyNavigation { get; set; }
    }
}