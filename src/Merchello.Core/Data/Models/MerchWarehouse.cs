namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchWarehouse
    {
        public MerchWarehouse()
        {
            this.MerchWarehouseCatalog = new HashSet<MerchWarehouseCatalog>();
        }

        public Guid Pk { get; set; }

        public string Name { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Locality { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public string CountryCode { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public bool IsDefault { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<MerchWarehouseCatalog> MerchWarehouseCatalog { get; set; }
    }
}