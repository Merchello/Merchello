using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.GatewayProviders;

namespace Merchello.Core.Services
{
    public class RegisteredGatewayProviderService : IRegisteredGatewayProviderService
    {
        public IRegisteredGatewayProvider GetByKey(Guid key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IRegisteredGatewayProvider> GetByGatewayProviderType(GatewayProviderType gatewayProviderType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IRegisteredGatewayProvider> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Save(IRegisteredGatewayProvider registeredGatewayProviderBase, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public void Delete(IRegisteredGatewayProvider registeredGatewayProviderBase)
        {
            throw new NotImplementedException();
        }
    }
}