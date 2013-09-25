using Merchello.Core.Services;

namespace Merchello.Core.Gateway
{
    internal sealed class GatewayContext : GatewayContextBase
    {
        internal GatewayContext(IRegisteredGatewayProviderService registeredGatewayProviderService) 
            : base(registeredGatewayProviderService)
        { }
      
    }
}