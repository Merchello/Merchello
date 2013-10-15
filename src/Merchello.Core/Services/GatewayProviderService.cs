using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Services
{
    public class GatewayProviderService : IGatewayProviderService
    {
        public IGatewayProvider GetByKey(Guid key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IGatewayProvider> GetByGatewayProviderType(GatewayProviderType gatewayProviderType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IGatewayProvider> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Save(IGatewayProvider gatewayProviderBase, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public void Delete(IGatewayProvider gatewayProviderBase)
        {
            throw new NotImplementedException();
        }
    }
}