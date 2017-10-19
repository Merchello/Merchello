namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed class ShipCountryDto
    {
        public ShipCountryDto()
        {
            this.MerchShipMethod = new HashSet<ShipMethodDto>();
        }

        public Guid Pk { get; set; }

        public Guid CatalogKey { get; set; }

        public string CountryCode { get; set; }

        public string Name { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<ShipMethodDto> MerchShipMethod { get; set; }

        public WarehouseCatalogDto CatalogDtoKeyNavigation { get; set; }
    }
}