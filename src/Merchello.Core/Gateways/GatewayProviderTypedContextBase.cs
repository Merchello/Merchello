using System;
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

            AssertProviderVersions();
        }

        /// <summary>
        /// Asserts the assembly versions get updated (if applicable) when the context is instantiated.
        /// </summary>
        /// TODO revist this.  Probably better to do something like this in the bootstrapper
        private void AssertProviderVersions()
        {
            var all = GetAllActivatedProviders().ToArray();
            var activated = GetAllActivatedProviders();

            foreach (var provider in activated)
            {
                var key = provider.Key;
                var resolved = all.FirstOrDefault(x => x.Key == key);

                if (resolved == null) continue;
                if (provider.TypeFullName.Equals(resolved.TypeFullName)) continue;
                
                provider.TypeFullName = resolved.TypeFullName;
                GatewayProviderService.Save(provider);
            }
        }

        /// <summary>
        /// Lists all actived <see cref="IGatewayProvider"/>
        /// </summary>
        /// <returns>A collection of all "activated" GatewayProvider of the particular type T</returns>
        public IEnumerable<IGatewayProvider> GetAllActivatedProviders()
        {
            return GatewayProviderResolver.GetActivatedProviders<T>();
        }

        /// <summary>
        /// Lists all available providers.  This list includes providers that are just resolved and not configured
        /// </summary>
        /// <returns>A collection of all Gatewayprovider</returns>
        public IEnumerable<IGatewayProvider> GetAllProviders()
        {
            return GatewayProviderResolver.GetAllProviders<T>();
        }

        /// <summary>
        /// Activates a <see cref="IGatewayProvider"/>
        /// </summary>
        /// <param name="gatewayProvider">The <see cref="IGatewayProvider"/> to be activated</param>
        public void ActivateProvider(IGatewayProvider gatewayProvider)
        {
            if(gatewayProvider.Activated) return;
            GatewayProviderService.Save(gatewayProvider);
            GatewayProviderResolver.RefreshCache();
        }

        /// <summary>
        /// Deactivates a <see cref="IGatewayProvider"/>
        /// </summary>
        /// <param name="gatewayProvider">The <see cref="IGatewayProvider"/> to be deactivated</param>
        public void DeactivateProvider(IGatewayProvider gatewayProvider)
        {
            if (!gatewayProvider.Activated) return;
            GatewayProviderService.Delete(gatewayProvider);
            GatewayProviderResolver.RefreshCache();
        }

        /// <summary>
        /// Resolves all active <see cref="IGatewayProvider"/>s of T
        /// </summary>
        /// <returns>A collection of all active TypedGatewayProviderinstances</returns>
        public abstract IEnumerable<T> CreateInstances();
    
        /// <summary>
        /// Resolves a <see cref="IGatewayProvider"/> by it's unique key
        /// </summary>
        /// <param name="key">The Guid 'key' of the provider</param>
        /// <returns>Returns a <see cref="IGatewayProvider"/> of type T</returns>
        public abstract T CreateInstance(Guid key);
        
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