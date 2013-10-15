using System.Collections.Generic;
using Merchello.Core.Gateway;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a the registered gateway provider base class
    /// </summary>
    public interface IGatewayProviderBase
    {
        Gateway.IGatewayProvider CreateInstance<T>(IDictionary<string, string> settings) where T : class, Gateway.IGatewayProvider;
    }
}
