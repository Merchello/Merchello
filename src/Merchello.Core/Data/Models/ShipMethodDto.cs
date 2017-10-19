namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed class ShipMethodDto
    {
        public ShipMethodDto()
        {
            this.MerchShipment = new HashSet<ShipmentDto>();
            this.MerchShipRateTier = new HashSet<ShipRateTierDto>();
        }

        public Guid Pk { get; set; }

        public string Name { get; set; }

        public Guid ShipCountryKey { get; set; }

        public Guid ProviderKey { get; set; }

        public decimal Surcharge { get; set; }

        public string ServiceCode { get; set; }

        public bool Taxable { get; set; }

        public string ProvinceData { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<ShipmentDto> MerchShipment { get; set; }

        public ICollection<ShipRateTierDto> MerchShipRateTier { get; set; }

        public GatewayProviderSettingsDto ProviderKeyNavigation { get; set; }

        public ShipCountryDto ShipCountryDtoKeyNavigation { get; set; }
    }
}