using System;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Services;
using Umbraco.Core;

namespace Merchello.Core
{
    /// <summary>
    /// Defines the MerchelloContext
    /// </summary>
    public interface IMerchelloContext : IDisposable
    {
        /// <summary>
        /// Umbraco's <see cref="CacheHelper"/>
        /// </summary>
        CacheHelper Cache { get; }

        /// <summary>
        /// The Merchello <see cref="IServiceContext"/>
        /// </summary>
        IServiceContext Services { get; }
        
        /// <summary>
        /// Gets the <see cref="IGatewayContext"/>
        /// </summary>
        IGatewayContext Gateways { get; }

        ///// <summary>
        ///// Gets the <see cref="ISalesManager"/>
        ///// </summary>
        //ISalesManager SalesManager { get; }

        /// <summary>
        /// Compares the binary version to that listed in the Merchello configuration to determine if the 
        /// package was upgraded
        /// </summary>
        bool IsConfigured { get; }

    }
}