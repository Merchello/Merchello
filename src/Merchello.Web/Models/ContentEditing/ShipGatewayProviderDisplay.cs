using System;
using System.Collections.Generic;

namespace Merchello.Web.Models.ContentEditing
{
    public class ShipGatewayProviderDisplay
    {
        public Guid Key { get; set; }
        public Guid ProviderTfKey { get; set; }
        public string Name { get; set; }
        public string TypeFullName { get; set; }
        //public ExtendedDataCollection ExtendedData { get; set; }
        public IEnumerable<ShipMethodDisplay> ShipMethods { get; set; }
    }
}
