namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchShipMethod
    {
        public MerchShipMethod()
        {
            this.MerchShipment = new HashSet<MerchShipment>();
            this.MerchShipRateTier = new HashSet<MerchShipRateTier>();
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

        public ICollection<MerchShipment> MerchShipment { get; set; }

        public ICollection<MerchShipRateTier> MerchShipRateTier { get; set; }

        public GatewayProviderSettingsDto ProviderKeyNavigation { get; set; }

        public MerchShipCountry ShipCountryKeyNavigation { get; set; }
    }
}