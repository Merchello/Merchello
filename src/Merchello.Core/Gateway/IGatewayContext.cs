using System;
using Merchello.Core.Models.GatewayProviders;

namespace Merchello.Core.Gateway
{
    public interface IGatewayContext
    {
        IRegisteredGatewayProvider GetByKey(Guid key);   
        void Refresh();
    }
}