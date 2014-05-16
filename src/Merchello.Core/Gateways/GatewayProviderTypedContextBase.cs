﻿using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Base class for GatewayContext objects
    /// </summary>
    public abstract class GatewayProviderTypedContextBase<T> : IGatewayProviderTypedContextBase<T>
        where T : GatewayProviderBase
    {
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly IGatewayProviderResolver _resolver;

        protected GatewayProviderTypedContextBase(IGatewayProviderService gatewayProviderService, IGatewayProviderResolver resolver)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");            
            Mandate.ParameterNotNull(resolver, "resolver");

            _gatewayProviderService = gatewayProviderService;            
            _resolver = resolver;

        }

        /// <summary>
        /// Lists all actived <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <returns>A collection of all "activated" GatewayProvider of the particular type T</returns>
        public IEnumerable<GatewayProviderBase> GetAllActivatedProviders()
        {
            return GatewayProviderResolver.GetActivatedProviders<T>();
        }

        /// <summary>
        /// Lists all available providers.  This list includes providers that are just resolved and not configured
        /// </summary>
        /// <returns>A collection of all Gatewayprovider</returns>
        public IEnumerable<GatewayProviderBase> GetAllProviders()
        {
            return GatewayProviderResolver.GetAllProviders<T>();
        }

        /// <summary>
        /// Instantiates a GatewayProvider given its registered Key
        /// </summary>
        /// <typeparam name="T">The Type of the GatewayProvider.  Must inherit from GatewayProviderBase</typeparam>
        /// <param name="gatewayProviderKey"></param>
        /// <param name="activatedOnly">Search only activated providers</param>
        /// <returns>An instantiated GatewayProvider</returns>
        public T GetProviderByKey(Guid gatewayProviderKey, bool activatedOnly = true)
        {
            return GatewayProviderResolver.GetProviderByKey<T>(gatewayProviderKey, activatedOnly);
        }

        /// <summary>
        /// Returns an instance of an 'active' GatewayProvider associated with a GatewayMethod based given the unique Key (Guid) of the GatewayMethod
        /// </summary>
        /// <param name="gatewayMethodKey">The unique key (Guid) of the <see cref="IGatewayMethod"/></param>
        /// <returns>An instantiated GatewayProvider</returns>
        public abstract T GetProviderByMethodKey(Guid gatewayMethodKey);

        /// <summary>
        /// Creates an instance GatewayProvider given its registered Key
        /// </summary>        
        [Obsolete("Use GetProviderByKey instead")]
        public T CreateInstance(Guid gatewayProviderKey)
        {
            return GetProviderByKey(gatewayProviderKey);
        }

        /// <summary>
        /// Activates a <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <param name="provider">The GatewayProvider to be activated</param>
        public void ActivateProvider(GatewayProviderBase provider)
        {
            ActivateProvider(provider.GatewayProviderSettings);
        }

        /// <summary>
        /// Activates a <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <param name="gatewayProviderSettings">The <see cref="IGatewayProviderSettings"/> to be activated</param>
        public void ActivateProvider(IGatewayProviderSettings gatewayProviderSettings)
        {

            if (gatewayProviderSettings.Activated) return;
            GatewayProviderService.Save(gatewayProviderSettings);
            GatewayProviderResolver.RefreshCache();
        }

        /// <summary>
        /// Deactivates a <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <param name="provider">The GatewayProvider to be deactivated</param>
        public void DeactivateProvider(GatewayProviderBase provider)
        {
            DeactivateProvider(provider.GatewayProviderSettings);
        }

        /// <summary>
        /// Deactivates a <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <param name="gatewayProviderSettings">The <see cref="IGatewayProviderSettings"/> to be deactivated</param>
        public void DeactivateProvider(IGatewayProviderSettings gatewayProviderSettings)
        {
            if (!gatewayProviderSettings.Activated) return;
            GatewayProviderService.Delete(gatewayProviderSettings);
            GatewayProviderResolver.RefreshCache();
        }


        /// <summary>
        /// Gets the <see cref="IGatewayProviderResolver"/>
        /// </summary>
        protected IGatewayProviderResolver GatewayProviderResolver
        {
            get{
            if(_resolver == null) throw new InvalidOperationException("GatewayProviderResolver has not been instantiated.");
                return _resolver;
            }
        }

        /// <summary>
        /// Gets the GatewayProviderService
        /// </summary>
        protected IGatewayProviderService GatewayProviderService
        {
            get { return _gatewayProviderService; }
        }

    }
}