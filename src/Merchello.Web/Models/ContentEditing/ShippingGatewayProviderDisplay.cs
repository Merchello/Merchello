using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public class ShippingGatewayProviderDisplay
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public string TypeFullName { get; set; }
        public ExtendedDataCollection ExtendedData { get; set; }  // TODO
        public IEnumerable<ShipMethodDisplay> ShipMethods { get; set; }
    }
}
