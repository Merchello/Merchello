using System;
using System.Collections.Generic;

namespace Merchello.Web.Models.ContentEditing
{
    public class ShipGatewayProviderDisplay
    {
        public Guid Key { get; set; }
        //public Guid ProviderTfKey { get; set; }   // Assume its a ship provider
        public string Name { get; set; }
        public string TypeFullName { get; set; }
        //public ExtendedDataCollection ExtendedData { get; set; }  // TODO
        public IEnumerable<ShipMethodDisplay> ShipMethods { get; set; }
    }
}
