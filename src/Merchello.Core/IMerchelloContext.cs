namespace Merchello.Core
{
    using System;
    using Gateways;
    using Services;
    using Umbraco.Core;

    /// <summary>
    /// Defines the MerchelloContext
    /// </summary>
    public interface IMerchelloContext : IDisposable
    {
        /// <summary>
        /// Gets the Umbraco <see cref="CacheHelper"/>
        /// </summary>
        CacheHelper Cache { get; }

        /// <summary>
        /// Gets the Merchello <see cref="IServiceContext"/>
        /// </summary>
        IServiceContext Services { get; }
        
        /// <summary>
        /// Gets the <see cref="IGatewayContext"/>
        /// </summary>
        IGatewayContext Gateways { get; }

        /// <summary>
        /// Gets a value indicating whether or not the Merchello needs to be upgraded
        /// </summary>
        /// <remarks>
        /// Compares the binary version to that listed in the Merchello configuration to determine if the 
        /// package was upgraded
        /// </remarks>
        bool IsConfigured { get; }

    }
}