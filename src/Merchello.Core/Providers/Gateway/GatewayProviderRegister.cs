using System;
using Merchello.Core.Models;
using Merchello.Core.Models.GatewayProviders;
using Merchello.Core.Services;

namespace Merchello.Core.Providers.Gateway
{
    internal sealed class GatewayProviderRegister : GatewayProviderRegisterBase
    {
        internal GatewayProviderRegister(IRegisteredGatewayProviderService registeredGatewayProviderService) 
            : base(registeredGatewayProviderService)
        { }
      
    }
}