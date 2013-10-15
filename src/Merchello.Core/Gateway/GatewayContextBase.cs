using System;
using System.Collections.Concurrent;

using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.Gateway
{
    internal abstract class GatewayContextBase : IGatewayContext
    {        
        private static readonly ConcurrentDictionary<Guid, IGatewayProviderBase> GatewayProviderCache = new ConcurrentDictionary<Guid, IGatewayProviderBase>();
        private IGatewayProviderService _gatewayProviderService;

        protected GatewayContextBase(IGatewayProviderService gatewayProviderService)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");

            _gatewayProviderService = gatewayProviderService;
        }

        public IGatewayProvider GetByKey(Guid key)
        {
            throw new NotImplementedException();
        }

        protected void AddToCache(Guid providerKey, IGatewayProviderBase gatewayProviderBase)
        {
            GatewayProviderCache.AddOrUpdate(providerKey, gatewayProviderBase, (x, y) => gatewayProviderBase);
        }
        
        public  void Refresh()
        {
            
        }
    }
}