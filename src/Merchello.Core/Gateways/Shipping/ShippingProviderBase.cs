using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Services;

namespace Merchello.Core.Gateways.Shipping
{
    public abstract class ShippingProviderBase<T> : GatewayProviderBase, IShippingGateway<T>
        where T : IGatewayShipMethod
    {        

        protected ShippingProviderBase(IMerchelloContext merchelloContext, IGatewayProvider gatewayProvider)
            : base(merchelloContext, gatewayProvider)
        { }

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IGatewayMethod> ListAvailableMethods();

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<T> ActiveShipMethods(IShipCountry shipCountry);


    }


}