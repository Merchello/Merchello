using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Web.Models;

namespace Merchello.Web.Gateways.Shipping
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