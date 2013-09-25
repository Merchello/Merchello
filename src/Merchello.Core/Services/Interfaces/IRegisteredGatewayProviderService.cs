using System;
using System.Collections;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.GatewayProviders;

namespace Merchello.Core.Services
{
    public interface IRegisteredGatewayProviderService
    {
        IRegisteredGatewayProvider GetByKey(Guid key);

        IEnumerable<IRegisteredGatewayProvider> GetByGatewayProviderType(GatewayProviderType gatewayProviderType);
            
        IEnumerable<IRegisteredGatewayProvider> GetAll();

        void Save(IRegisteredGatewayProvider registeredGatewayProviderBase, bool raiseEvents = true);

        void Delete(IRegisteredGatewayProvider registeredGatewayProviderBase);
    }
}