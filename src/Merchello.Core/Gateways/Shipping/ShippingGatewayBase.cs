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
    /// <typeparam name="T">Must implement <see cref="IGatewayShipMethod"/></typeparam>
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
        /// Creates an instance of a ship method (T) without persisting it to the database
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// ShipMethods should be unique with respect to <see cref="IShipCountry"/> and <see cref="IGatewayResource"/>
        /// 
        /// </remarks>
        public abstract T CreateShipMethod(IGatewayResource gatewayResource, IShipCountry shipCountry, string name);
        

        /// <summary>
        /// Saves a shipmethod
        /// </summary>
        /// <param name="shipMethod"></param>
        public abstract void SaveShipMethod(T shipMethod);

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


        //public static ShippingGatewayBase<T> Instance()
        //{
            
        //}

    }


}