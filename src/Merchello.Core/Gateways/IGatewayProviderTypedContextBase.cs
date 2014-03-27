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
        /// Lists all available <see cref="IGatewayProvider"/>
        /// </summary>
        /// <returns>A collection of all GatewayProvider of the particular type T</returns>
        IEnumerable<IGatewayProvider> GetAllActivatedProviders(); 
            
        /// <summary>
        /// Resolves all active <see cref="IGatewayProvider"/>s of T
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> CreateInstances();

        /// <summary>
        /// Resolves a <see cref="IGatewayProvider"/> by it's unique key
        /// </summary>
        /// <param name="key">The Guid 'key' of the provider</param>
        /// <returns>Returns a <see cref="IGatewayProvider"/> of type T</returns>
        T CreateInstance(Guid key);
    }
}