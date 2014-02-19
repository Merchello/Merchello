using System;
using System.Collections;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Defines a GatewayContext for a specific provider type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGatewayProviderTypedContextBase<out T> where T : GatewayProviderBase
    {
        /// <summary>
        /// Resolves all active <see cref="IGatewayProvider"/>s of T
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> ResolveAllActiveProviders();

        /// <summary>
        /// Resolves a <see cref="IGatewayProvider"/> by it's unique key
        /// </summary>
        /// <param name="key">The Guid 'key' of the provider</param>
        /// <returns>Returns a <see cref="IGatewayProvider"/> of type T</returns>
        T ResolveByKey(Guid key);
    }
}