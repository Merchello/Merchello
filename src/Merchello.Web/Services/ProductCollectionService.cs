namespace Merchello.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Services;
    using Merchello.Web.Models.Ui.Rendering;

    /// <summary>
    /// A service responsible for retrieving <see cref="ProductCollection"/>.
    /// </summary>
    internal class ProductCollectionService : EntityCollectionProxyServiceBase, IProductCollectionService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCollectionService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public ProductCollectionService(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
        }

        /// <summary>
        /// Gets a <see cref="IProductCollection"/> by it's Key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductCollection"/>.
        /// </returns>
        public IProductCollection GetByKey(Guid key)
        {
            var collection = Service.GetByKey(key);
            if (collection == null) return null;

            return collection.EntityTfKey != Constants.TypeFieldKeys.Entity.ProductKey 
                ? null : 
                Map(collection);
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductCollection}"/>.
        /// </returns>
        public IEnumerable<IProductCollection> GetAll(params Guid[] keys)
        {
            return Map(Service.GetAll(keys));
        }

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
        public IEnumerable<IProductCollection> GetCollectionsContainingProduct(Guid productKey)
        {
            var cacheKey = string.Format("{0}.productcollectionscontaining", productKey);
            var pc = Cache.GetCacheItem(cacheKey);
            if (pc != null) return (IEnumerable<IProductCollection>)pc;
            return
                (IEnumerable<IProductCollection>)
                Cache.GetCacheItem(
                    cacheKey,
                    () => Map(((EntityCollectionService)Service).GetEntityCollectionsByProductKey(productKey, false)));
        }

        /// <summary>
        /// Gets the child collections of the collection with key passed as parameter.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductCollection}"/>.
        /// </returns>
        public IEnumerable<IProductCollection> GetChildCollections(Guid collectionKey)
        {
            var cacheKey = string.Format("{0}.productcollectionchildren", collectionKey);

            var collections = (IEnumerable<IProductCollection>)Cache.GetCacheItem(cacheKey);
            if (collections != null) return collections;

            return
                (IEnumerable<IProductCollection>)
                Cache.GetCacheItem(cacheKey, () => Map(((EntityCollectionService)Service).GetChildren(collectionKey)));
        }

        /// <summary>
        /// Maps <see cref="IEntityCollection"/> to <see cref="IProductCollection"/>.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="IProductCollection"/>.
        /// </returns>
        private static IProductCollection Map(IEntityCollection collection)
        {
            return collection.EntityTfKey == Constants.TypeFieldKeys.Entity.ProductKey ?
                new ProductCollection(collection) :
                null;
        }

        /// <summary>
        /// Maps a collection of <see cref="IEntityCollection"/> to <see cref="IProductCollection"/>.
        /// </summary>
        /// <param name="collections">
        /// The collections.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductCollection}"/>.
        /// </returns>
        private static IEnumerable<IProductCollection> Map(IEnumerable<IEntityCollection> collections)
        {
            return collections.Where(x => x != null).Select(Map);
        }
    }
}