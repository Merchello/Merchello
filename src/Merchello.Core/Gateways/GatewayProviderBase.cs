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
        private readonly IGatewayProviderSettings _gatewayProviderSettings;
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly IRuntimeCacheProvider _runtimeCache;

        protected GatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(gatewayProviderSettings, "gatewayProvider");
            Mandate.ParameterNotNull(runtimeCacheProvider, "runtimeCacheProvider");

            _gatewayProviderService = gatewayProviderService;
            _gatewayProviderSettings = gatewayProviderSettings;
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
            get { return _gatewayProviderSettings.Key; }
        }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderService"/>
        /// </summary>
        public IGatewayProviderService GatewayProviderService
        {
            get { return _gatewayProviderService; }
        }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderSettings"/>
        /// </summary>
        public virtual IGatewayProviderSettings GatewayProviderSettings 
        {
            get { return _gatewayProviderSettings; }
        }

        /// <summary>
        /// Gets the ExtendedData collection from the <see cref="IGatewayProviderSettings"/>
        /// </summary>
        public virtual ExtendedDataCollection ExtendedData
        {
            get { return _gatewayProviderSettings.ExtendedData; }
        }

        /// <summary>
        /// Gets the "activated property" from the <see cref="IGatewayProviderSettings"/>
        /// </summary>
        public virtual bool Activated
        {
            get { return _gatewayProviderSettings.Activated; }
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