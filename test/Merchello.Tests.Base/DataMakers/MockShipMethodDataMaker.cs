using System;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Tests.Base.DataMakers
{
    /// <summary>
    /// Helper class to assist in putting together ship method data for testing
    /// </summary>
    public class MockShipMethodDataMaker : MockDataMakerBase
    {

        public static IShipMethod FlatRateShipMethodMockForInserting()
        {
            return new ShipMethod()
                {
                    Name = "Flat Rate",
                    ProviderKey = Guid.NewGuid(),
                    ShipMethodTfKey = new ShipMethodTypeField().GetTypeField(ShipMethodType.FlatRate).TypeKey,
                    Surcharge = 0M,
                    ServiceCode = string.Empty
                };
        }

        public static IShipMethod CarrierMockForInserting()
        {
            return new ShipMethod()
            {
                Name = "Carrier Shipping Supreme",
                ProviderKey = Guid.NewGuid(),
                ShipMethodTfKey = new ShipMethodTypeField().GetTypeField(ShipMethodType.Carrier).TypeKey,
                Surcharge = 1M,
                ServiceCode = "CSS"
            };
        }

    }
}