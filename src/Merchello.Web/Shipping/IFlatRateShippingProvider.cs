using System.Collections.Generic;

namespace Merchello.Web.Shipping
{
    public interface IFlatRateShippingProvider : IShippingGatewayProvider
    {
            IEnumerable<IRateTableShipMethod> ShipMethods { get; set; }
    }

    public interface IRateTableShipMethod
    {
        IShipRateTable RateTable { get;  }
    }
}