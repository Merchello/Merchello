namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Models;

    /// <summary>
    /// The EntityProxyService interface.
    /// </summary>
    /// <typeparam name="TProxy">
    /// The type of entity
    /// </typeparam>
    public interface IEntityProxyQuery<out TProxy> : IEntityProxyQuery
        where TProxy : IEntityProxy
    {
        /// <summary>
        /// Gets a collection of provider responsible for managing entity collections that can be queries by this service.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IProviderInfo}"/>.
        /// </returns>
        IEnumerable<IProviderMeta> GetCollectionProviders();

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
    public interface IEntityProxyQuery
    {
    }
}