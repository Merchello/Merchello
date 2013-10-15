using System;
using System.Collections;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Services
{
    public interface IGatewayProviderService
    {
        IGatewayProvider GetByKey(Guid key);

        IEnumerable<IGatewayProvider> GetByGatewayProviderType(GatewayProviderType gatewayProviderType);
            
        IEnumerable<IGatewayProvider> GetAll();

        void Save(IGatewayProvider gatewayProviderBase, bool raiseEvents = true);

        void Delete(IGatewayProvider gatewayProviderBase);
    }
}