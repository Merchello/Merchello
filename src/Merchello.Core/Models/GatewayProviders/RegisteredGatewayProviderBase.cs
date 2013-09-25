using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models.GatewayProviders
{
    public abstract class RegisteredGatewayProviderBase : KeyEntity, IRegisteredGatewayProviderBase
    {
        // TODO : RSS refactor this to use an Attempt
        public abstract IGatewayProvider CreateInstance<T>(IDictionary<string, string> settings) where T : class, IGatewayProvider;

    }
}