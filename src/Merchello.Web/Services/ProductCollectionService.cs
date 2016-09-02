namespace Merchello.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.DataStructures;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Services;
    using Merchello.Web.Models.Ui.Rendering;

    using Umbraco.Core.Cache;

    /// <summary>
    /// A service responsible for retrieving <see cref="ProductCollection"/>.
    /// </summary>
    internal class ProductCollectionService : EntityCollectionProxyServiceBase, IProductCollectionService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCollectionService"/> class.
        /// </summary>
        /// <param name="entityCollectionService">
        /// The entity collection service.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        public ProductCollectionService(IEntityCollectionService entityCollectionService, ICacheProvider cache)
            : base(entityCollectionService, cache)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCollectionService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        internal ProductCollectionService(IMerchelloContext merchelloContext)
            : this(merchelloContext.Services.EntityCollectionService, merchelloContext.Cache.RequestCache)
        {
        }

        /// <summary>
        /// Gets the root level collections.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IProductCollection}"/>.
        /// </returns>
        public IEnumerable<IProductCollection> GetRootLevelCollections()
        {
            var cacheKey = GetCacheKey("GetRootLevelCollections");
            var pc = Cache.GetCacheItem(cacheKey);
            if (pc != null) return (IEnumerable<IProductCollection>)pc;

            // REFACTOR we need more specific queries that create a distinction between filters and collections
            // The service call may return filter collections too if they don't have parents
            // but they will get removed in the mapping.
            return (IEnumerable<IProductCollection>)
                    Cache.GetCacheItem(
                        cacheKey, 
                        () => Map(Service.GetRootLevelEntityCollections()));
        }

        /// <summary>
        /// Gets the root level collections as trees.
        /// </summary>
        /// <returns>
        /// The collection <see cref="TreeNode{IProductCollection}"/>.
        /// </returns>
        public IEnumerable<TreeNode<IProductCollection>> GetRootLevelCollectionTrees()
        {
            var cacheKey = GetCacheKey("GetRootLevelCollectionTrees");
            var trees = (IEnumerable<TreeNode<IProductCollection>>)Cache.GetCacheItem(cacheKey);
            if (trees != null) return trees;

            var collections = GetAll().ToArray();
            var tl = collections
                        .Where(x => x.ParentKey == null)
                        .Select(root => new TreeNode<IProductCollection>(root)
                        .Load(collections));

            return (IEnumerable<TreeNode<IProductCollection>>)Cache.GetCacheItem(cacheKey, () => tl);
        }

        /// <summary>
        /// Gets a collection of provider responsible for managing entity collections that can be queries by this service.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IProviderInfo}"/>.
        /// </returns>
        public IEnumerable<IProviderMeta> GetProviders()
        {
            var atts = EntityCollectionProviderResolver.Current.GetProviderAttributes<IProductCollectionProvider>();

            return atts.Select(x => new ProviderMeta(x));
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
            var cacheKey = GetCacheKey("GetAll", keys);
            var pc = Cache.GetCacheItem(cacheKey);
            if (pc != null) return (IEnumerable<IProductCollection>)pc;

            return
                 (IEnumerable<IProductCollection>)
                    Cache.GetCacheItem(
                        cacheKey,
                        () => Map(Service.GetAll(keys)));
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
            var cacheKey = GetCacheKey("GetCollectionsContainingProduct", productKey);
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
            var cacheKey = GetCacheKey("GetChildCollections", collectionKey);

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
            return collection.EntityTfKey == Constants.TypeFieldKeys.Entity.ProductKey && !collection.IsFilter ?
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
            return collections.Select(Map).Where(x => x != null);
        }

        /// <summary>
        /// Ensures a tree is created in request cache.
        /// </summary>
        /// <param name="all">
        /// The all.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductCollection}"/>.
        /// </returns>
        private IEnumerable<TreeNode<IProductCollection>> BuildTrees(IEnumerable<IProductCollection> all)
        {
            var cacheKey = GetCacheKey("BuildTrees");
            var trees = (IEnumerable<TreeNode<IProductCollection>>)Cache.GetCacheItem(cacheKey);
            if (trees != null) return trees;

            var collections = all as IProductCollection[] ?? all.ToArray();
            var tl = collections
                        .Where(x => x.ParentKey == null)
                        .Select(root => new TreeNode<IProductCollection>(root)
                        .Load(collections));

            return (IEnumerable<TreeNode<IProductCollection>>)Cache.GetCacheItem(cacheKey, () => tl);
        }
    }
}