using System.Collections.Generic;
using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Services;

namespace Merchello.Web.Shipping.Gateway
{
    public abstract class ShippingGatewayBase<T> : GatewayBase, IShippingGatewayProvider<T>
        where T : IGatewayShipMethod
    {
        private readonly IShippingService _shippingService;

        protected ShippingGatewayBase(IMerchelloContext merchelloContext, IGatewayProvider gatewayProvider)
            : base(gatewayProvider)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");

            _shippingService = merchelloContext.Services.ShippingService;
        }

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IGatewayMethod> ListAvailableMethods();
    
        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<T> GetActiveShipMethods(IShipCountry shipCountry);
    }
}