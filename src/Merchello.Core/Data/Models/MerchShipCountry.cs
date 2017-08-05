namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchShipCountry
    {
        public MerchShipCountry()
        {
            this.MerchShipMethod = new HashSet<MerchShipMethod>();
        }

        public Guid Pk { get; set; }

        public Guid CatalogKey { get; set; }

        public string CountryCode { get; set; }

        public string Name { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<MerchShipMethod> MerchShipMethod { get; set; }

        public MerchWarehouseCatalog CatalogKeyNavigation { get; set; }
    }
}