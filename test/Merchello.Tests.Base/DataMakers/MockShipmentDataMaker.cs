using System;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.DataMakers
{
    public class MockShipmentDataMaker : MockDataMakerBase
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

        public static IShipMethod MockShipMethodForInserting()
        {
            return new ShipMethod()
                {
                    Name = "Mock Ship Method",
                    ProviderKey = Guid.NewGuid(),
                    ShipMethodTypeFieldKey = TypeFields.TypeFieldMock.ShipMethodCarrier.TypeKey,
                    Surcharge = 0m,
                    ServiceCode = "Mock"
                };
        }
    }
}
