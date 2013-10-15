using System.Collections.Generic;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    public abstract class GatewayProviderBase : KeyEntity, IGatewayProviderBase
    {
        // TODO : RSS refactor this to use an Attempt
        public abstract Gateway.IGatewayProvider CreateInstance<T>(IDictionary<string, string> settings) where T : class, Gateway.IGatewayProvider;

    }
}