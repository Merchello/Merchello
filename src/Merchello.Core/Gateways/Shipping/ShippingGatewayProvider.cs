using System;
using System.Collections.Generic;
using System.ComponentModel;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Defines the Shipping Gateway abstract class
    /// </summary>
    public abstract class ShippingGatewayProvider : GatewayProviderBase, IShippingGatewayProvider        
    {
        private readonly IRuntimeCacheProvider _runtimeCache;

        protected ShippingGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProvider)
        {
            _runtimeCache = runtimeCacheProvider;
        }

        /// <summary>
        /// Creates an instance of a ship method (T) without persisting it to the database
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// ShipMethods should be unique with respect to <see cref="IShipCountry"/> and <see cref="IGatewayResource"/>
        /// 
        /// </remarks>
        public abstract IGatewayShipMethod CreateShipMethod(IGatewayResource gatewayResource, IShipCountry shipCountry, string name);
        

        /// <summary>
        /// Saves a shipmethod
        /// </summary>
        /// <param name="shipMethod"></param>
        public abstract void SaveShipMethod(IGatewayShipMethod shipMethod);

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IGatewayResource> ListAvailableMethods();

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IGatewayShipMethod> ActiveShipMethods(IShipCountry shipCountry);

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