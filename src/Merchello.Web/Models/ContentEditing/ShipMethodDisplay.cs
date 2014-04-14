using System;
using System.Collections.Generic;

namespace Merchello.Web.Models.ContentEditing
{
    public class ShipMethodDisplay : DialogEditorDisplayBase
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public Guid ProviderKey { get; set; }
        public Guid ShipCountryKey { get; set; }
        public decimal Surcharge { get; set; }
        public string ServiceCode { get; set; }
        public bool Taxable { get; set; }
        public IEnumerable<ShipProvinceDisplay> Provinces { get; set; }
    }
}
