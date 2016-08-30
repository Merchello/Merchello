namespace Merchello.Web.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Models.Ui.Rendering;

    /// <summary>
    /// The EntityProxyService interface.
    /// </summary>
    /// <typeparam name="TProxy">
    /// The type of entity
    /// </typeparam>
    public interface IEntityProxyService<out TProxy> : IEntityProxyService
        where TProxy : IEntityProxy
    {
        /// <summary>
        /// Gets the proxy entity by it's Key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The proxy object.
        /// </returns>
        TProxy GetByKey(Guid key);

        /// <summary>
        /// The get all.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The collection of proxies.
        /// </returns>
        IEnumerable<TProxy> GetAll(params Guid[] keys);
    }

    /// <summary>
    /// Marker interface for entity proxy services.
    /// </summary>
    public interface IEntityProxyService
    {
    }
}