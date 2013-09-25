using System;
using Merchello.Core.Models;
using Merchello.Core.Models.GatewayProviders;

namespace Merchello.Core.Providers.Gateway
{
    public interface IGatewayProviderRegister
    {
        IRegisteredGatewayProviderBase GetByKey(Guid key);   
        void Refresh();
    }
}