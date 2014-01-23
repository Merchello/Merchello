using System;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Represents a GatewayProviderFactory
    /// </summary>
    internal class GatewayProviderFactory : IGatewayProviderFactory
    {
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly IRuntimeCacheProvider _runtimeCache;

        public GatewayProviderFactory(IGatewayProviderService gatewayProviderService,
                                      IRuntimeCacheProvider runtimeCacheProvider)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(runtimeCacheProvider, "runtimeCacheProvider");

            _gatewayProviderService = gatewayProviderService;
            _runtimeCache = runtimeCacheProvider;
        }
       
        /// <summary>
        /// Returns a typed instance of a <see cref="GatewayProviderBase"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public T GetInstance<T>(IGatewayProvider provider) where T : GatewayProviderBase
        {
            return ActivateGateway(provider) as T;
        }

        /// <summary>
        /// Creates an instance of a <see cref="GatewayProviderBase"/>
        /// </summary>
        /// <param name="gatewayProvider"></param>
        /// <returns></returns>
        private GatewayProviderBase ActivateGateway(IGatewayProvider gatewayProvider)
        {
            var ctorArgs = new[] { typeof(IGatewayProviderService), typeof(IGatewayProvider), typeof(IRuntimeCacheProvider) };
            var ctoArgValues = new object[] { _gatewayProviderService, gatewayProvider, _runtimeCache };
            return ActivatorHelper.CreateInstance<GatewayProviderBase>(Type.GetType(gatewayProvider.TypeFullName), ctorArgs, ctoArgValues);
        }

        internal IGatewayProviderService GatewayProviderService
        {
            get { return _gatewayProviderService; }
        }
    }
}