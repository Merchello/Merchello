using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.Data
{
    public class ShipmentData
    {
        public static IShipment MockShipmentForInserting(int invoiceId, int? shipMethodId)
        {
            return new Shipment(invoiceId)
                {
                    Address1 = "114 W. Magnolia St.",
                    Address2 = "Suite 504",
                    Locality = "Bellingham",
                    Region = "WA",
                    CountryCode = "US",
                    PostalCode = "98225",
                    Phone = "555-555-5555",
                    ShipMethodId = shipMethodId
                };
        }
    }
}
