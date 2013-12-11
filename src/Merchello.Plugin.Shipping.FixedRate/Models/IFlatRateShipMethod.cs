using Merchello.Core.Models;

namespace Merchello.Plugin.Shipping.FixedRate.Models
{
    public interface IFlatRateShipMethod : IShipMethod
    {
         ShipRateTable ShipRateTable { get;  }
    }

    public class ShipRateTable
    {
    }
}