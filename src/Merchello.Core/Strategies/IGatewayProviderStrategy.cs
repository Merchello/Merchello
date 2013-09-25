using System;
using Merchello.Core.Gateway;

namespace Merchello.Core.Strategies
{
    /// <summary>
    /// Marker interface for gateway provider strategies
    /// </summary>
    public interface IGatewayProviderStrategy : IStrategy, IDisposable
    {
        IGatewayResponse Send();
    }
}
