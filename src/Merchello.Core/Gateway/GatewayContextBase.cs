using System;
using System.Collections.Concurrent;
using Merchello.Core.Models.GatewayProviders;
using Merchello.Core.Services;

namespace Merchello.Core.Gateway
{
    internal abstract class GatewayContextBase : IGatewayContext
    {        
        private static readonly ConcurrentDictionary<Guid, IRegisteredGatewayProviderBase> GatewayProviderCache = new ConcurrentDictionary<Guid, IRegisteredGatewayProviderBase>();
        private IRegisteredGatewayProviderService _registeredGatewayProviderService;

        protected GatewayContextBase(IRegisteredGatewayProviderService registeredGatewayProviderService)
        {
            Mandate.ParameterNotNull(registeredGatewayProviderService, "gatewayProviderService");

            _registeredGatewayProviderService = registeredGatewayProviderService;
        }

        public IRegisteredGatewayProvider GetByKey(Guid key)
        {
            throw new NotImplementedException();
        }

        protected void AddToCache(Guid providerKey, IRegisteredGatewayProviderBase registeredGatewayProviderBase)
        {
            GatewayProviderCache.AddOrUpdate(providerKey, registeredGatewayProviderBase, (x, y) => registeredGatewayProviderBase);
        }
        
        public  void Refresh()
        {
            
        }
    }
}