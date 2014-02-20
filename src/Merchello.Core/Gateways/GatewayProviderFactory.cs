using System;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

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
            return CreateGatewayProviderInstance(provider) as T;
        }

        /// <summary>
        /// Creates an instance of a <see cref="GatewayProviderBase"/>
        /// </summary>
        /// <param name="gatewayProvider"></param>
        /// <returns></returns>
        private GatewayProviderBase CreateGatewayProviderInstance(IGatewayProvider gatewayProvider)
        {
            var ctoArgValues = new object[] { _gatewayProviderService, gatewayProvider, _runtimeCache };
            var attempt = ActivatorHelper.CreateInstance<GatewayProviderBase>(gatewayProvider.TypeFullName, ctoArgValues);

            if (!attempt.Success)
            {
                LogHelper.Error<GatewayProviderBase>("PackageBasket failed to instantiate the defaultStrategy.", attempt.Exception);
                throw attempt.Exception;
            }

            return attempt.Result;
        }

        internal IGatewayProviderService GatewayProviderService
        {
            get { return _gatewayProviderService; }
        }
    }
}