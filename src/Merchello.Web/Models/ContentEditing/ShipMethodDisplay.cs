using System;
using System.Collections.Generic;

namespace Merchello.Web.Models.ContentEditing
{
    public class ShipMethodDisplay
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public Guid WarehouseCountryKey { get; set; }
        public Guid ProviderKey { get; set; }
        public Guid ShipMethodTfKey { get; set; }
        public decimal Surcharge { get; set; }
        public string ServiceCode { get; set; }
        public IEnumerable<ShipProvinceDisplay> Provinces { get; set; }
    }
}
