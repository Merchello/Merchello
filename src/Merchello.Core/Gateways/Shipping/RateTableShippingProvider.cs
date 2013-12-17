using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Gateways.Shipping
{
    public class RateTableShippingProvider : ShippingProviderBase<IRateTableShipMethod>
    {
        #region "Available Methods"
        
        private static readonly IEnumerable<IGatewayMethod> AvailableMethods  = new List<IGatewayMethod>()
            {
                new GatewayMethod("VaryByWeight", "Vary by Weight"),
                new GatewayMethod("PercentTotal", "Percentage of Total")
            };

        #endregion


        public RateTableShippingProvider(IMerchelloContext merchelloContext, IGatewayProvider gatewayProvider) 
            : base(merchelloContext, gatewayProvider)
        { }


        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IGatewayMethod> ListAvailableMethods()
        {
            return AvailableMethods;
        }

        public override IEnumerable<IRateTableShipMethod> ActiveShipMethods(IShipCountry shipCountry)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IRateTableShipMethod : IGatewayShipMethod
    {
        IShipRateTable RateTable { get; }
    }
}