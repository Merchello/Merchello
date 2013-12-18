using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Shipping
{
    public abstract class ShippingGatewayBase<T> : GatewayBase, IShippingGatewayBase<T>
        where T : IGatewayShipMethod
    {
        private readonly IRuntimeCacheProvider _runtimeCache;

        protected ShippingGatewayBase(IMerchelloContext merchelloContext, IGatewayProvider gatewayProvider)
            : base(merchelloContext, gatewayProvider)
        {
            _runtimeCache = MerchelloContext.Cache.RuntimeCache;
        }

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IGatewayResource> ListAvailableMethods();

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<T> ActiveShipMethods(IShipCountry shipCountry);

        /// <summary>
        /// Gets the RuntimeCache
        /// </summary>
        /// <returns></returns>
        protected IRuntimeCacheProvider RuntimeCache
        {
            get { return _runtimeCache; }
        }
    }


}