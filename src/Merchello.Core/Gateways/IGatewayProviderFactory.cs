using Merchello.Core.Models;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Defines a GatewayProviderFactory
    /// </summary>
    internal interface IGatewayProviderFactory
    {
        /// <summary>
        /// Returns an instance of a GatewayProvider
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        T GetInstance<T>(IGatewayProvider provider) where T : GatewayProviderBase;
    }
}