namespace Merchello.Web.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Models.Ui.Rendering;

    /// <summary>
    /// Defines a service responsible for <see cref="IProductCollection"/>.
    /// </summary>
    public interface IProductCollectionService : IEntityProxyService<IProductCollection>
    {
        /// <summary>
        /// Gets the root level collections.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IProductCollection}"/>.
        /// </returns>
        IEnumerable<IProductCollection> GetRootLevelCollections();

        /// <summary>
        /// Get collections containing product.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductCollection}"/>.
        /// </returns>
        /// TODO the Core service has inconsistent naming
        IEnumerable<IProductCollection> GetCollectionsContainingProduct(Guid productKey);

        /// <summary>
        /// Gets the child collections of the collection with key passed as parameter.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductCollection}"/>.
        /// </returns>
        IEnumerable<IProductCollection> GetChildCollections(Guid collectionKey);

        /// <summary>
        /// Get collections not containing a product.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductCollection}"/>.
        /// </returns>
        /// TODO the Core sercvice does not implement this method but does for filters (inconsistent)
        // IEnumerable<IProductCollection> GetCollectionsNotContainingProduct(Guid productKey);
    }
}