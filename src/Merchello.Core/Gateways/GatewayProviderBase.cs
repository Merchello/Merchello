using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Defines the GatewayBase
    /// </summary>
    public abstract class GatewayProviderBase : IProvider
    {        
        private readonly IGatewayProviderSetting _gatewayProviderSetting;
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly IRuntimeCacheProvider _runtimeCache;

        protected GatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProviderSetting gatewayProviderSetting, IRuntimeCacheProvider runtimeCacheProvider)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(gatewayProviderSetting, "gatewayProvider");
            Mandate.ParameterNotNull(runtimeCacheProvider, "runtimeCacheProvider");

            _gatewayProviderService = gatewayProviderService;
            _gatewayProviderSetting = gatewayProviderSetting;
            _runtimeCache = runtimeCacheProvider;
        }

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="IGatewayResource"/></returns>
        public abstract IEnumerable<IGatewayResource> ListResourcesOffered();


        // The properties Name and Key will be likely become attribute defined properties
        // TODO enable devs to define each of these values. 
        
        ///// <summary>
        ///// The name of the GatewayProvider
        ///// </summary>
        //public abstract string Name { get; }

        /// <summary>
        /// The unique Key that will be used
        /// </summary>
        public Guid Key 
        {
            get { return _gatewayProviderSetting.Key; }
        }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderService"/>
        /// </summary>
        public IGatewayProviderService GatewayProviderService
        {
            get { return _gatewayProviderService; }
        }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderSetting"/>
        /// </summary>
        public virtual IGatewayProviderSetting GatewayProviderSetting 
        {
            get { return _gatewayProviderSetting; }
        }

        /// <summary>
        /// Gets the ExtendedData collection from the <see cref="IGatewayProviderSetting"/>
        /// </summary>
        public virtual ExtendedDataCollection ExtendedData
        {
            get { return _gatewayProviderSetting.ExtendedData; }
        }

        /// <summary>
        /// Gets the "activated property" from the <see cref="IGatewayProviderSetting"/>
        /// </summary>
        public virtual bool Activated
        {
            get { return _gatewayProviderSetting.Activated; }
        }

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