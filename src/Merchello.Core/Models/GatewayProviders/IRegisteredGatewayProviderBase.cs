using System;
using System.Collections;
using System.Collections.Generic;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models.GatewayProviders
{
    /// <summary>
    /// Defines a the registered gateway provider base class
    /// </summary>
    public interface IRegisteredGatewayProviderBase
    {
        IGatewayProvider CreateInstance<T>(IDictionary<string, string> settings) where T : class, IGatewayProvider;
    }
}
