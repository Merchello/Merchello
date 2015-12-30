namespace Merchello.Core.Services
{
    using System;

    using Umbraco.Core.Services;

    /// <summary>
    /// Defines a Merchello service used by Umbraco as a Service Item Providers.
    /// </summary>
    public interface IUaaSItemProviderService : IService
    {
        /// <summary>
        /// The exists.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Exists(Guid key);
    }
}