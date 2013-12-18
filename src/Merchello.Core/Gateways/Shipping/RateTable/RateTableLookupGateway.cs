using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Umbraco.Core;

namespace Merchello.Core.Gateways.Shipping.RateTable
{
    /// <summary>
    /// Defines the RateTableLookupGateway
    /// </summary>
    public class RateTableLookupGateway : ShippingGatewayBase<IRateTableShipMethod>
    {
        #region "Available Methods"
        
        private static readonly IEnumerable<IGatewayResource> AvailableMethods  = new List<IGatewayResource>()
            {
                new GatewayResource("VBW", "Vary by Weight"),
                new GatewayResource("POT", "Percentage of Total")
            };

        #endregion


        public RateTableLookupGateway(IMerchelloContext merchelloContext, IGatewayProvider gatewayProvider)
            : base(merchelloContext, gatewayProvider)
        { }

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IGatewayResource> ListAvailableMethods()
        {
            return AvailableMethods;
        }

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IRateTableShipMethod> ActiveShipMethods(IShipCountry shipCountry)
        {
            var methods = MerchelloContext.Services.GatewayProviderService.GetGatewayProviderShipMethods(GatewayProvider.Key, shipCountry.Key);
            return methods
                .Select(
                shipMethod => new RateTableShipMethod(AvailableMethods.FirstOrDefault(x => x.ServiceCode == shipMethod.ServiceCode), shipMethod)
                ).OrderBy(x => x.ShipMethod.Name);
        }
    }
}