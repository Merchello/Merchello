namespace Merchello.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Services;
    using Merchello.Web.Models.Ui.Rendering;

    using Umbraco.Core.Cache;

    /// <summary>
    /// Represents a ProductFilterGroupService.
    /// </summary>
    internal class ProductFilterGroupService : EntityCollectionProxyServiceBase, IProductFilterGroupService
    {
        /// <summary>
        /// Collection provider keys that designate a collection is a filter.
        /// </summary>
        private Guid[] _filterProviderKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFilterGroupService"/> class.
        /// </summary>
        /// <param name="entityCollectionService">
        /// The  <see cref="IEntityCollectionService"/>.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        public ProductFilterGroupService(IEntityCollectionService entityCollectionService, ICacheProvider cache)
            : this(entityCollectionService, cache, EntityCollectionProviderResolver.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFilterGroupService"/> class.
        /// </summary>
        /// <param name="entityCollectionService">
        /// The <see cref="IEntityCollectionService"/>.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="resolver">
        /// The resolver.
        /// </param>
        public ProductFilterGroupService(IEntityCollectionService entityCollectionService, ICacheProvider cache, EntityCollectionProviderResolver resolver)
            : base(entityCollectionService, cache)
        {
            this.Initialize(resolver);
        }

        /// <summary>
        /// Gets a collection of provider responsible for managing entity collections that can be queries by this service.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IProviderInfo}"/>.
        /// </returns>
        public IEnumerable<IProviderMeta> GetProviders()
        {
            var atts = EntityCollectionProviderResolver.Current.GetProviderAttributes<IProductFilterGroupProvider>();

            return atts.Select(x => new ProviderMeta(x));
        }

        /// <summary>
        /// Gets an <see cref="IProductFilterGroup"/>.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductFilterGroup"/>.
        /// </returns>
        public IProductFilterGroup GetByKey(Guid key)
        {
            return Map(((EntityCollectionService)Service).GetEntityFilterGroup(key));
        }

        /// <summary>
        /// Gets all of the <see cref="IProductFilterGroup"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductFilterGroup}"/>.
        /// </returns>
        public IEnumerable<IProductFilterGroup> GetAll(params Guid[] keys)
        {
            var collections = ((EntityCollectionService)Service).GetEntityFilterGroupsByProviderKeys(_filterProviderKeys);

            return Map(keys.Any() ? 
                            collections.Where(x => keys.Any(y => y == x.Key)) : 
                            collections);
        }

        /// <summary>
        /// Gets a collection of <see cref="IProductFilterGroup"/> that has at least one filter that contains a product with key passed as parameter.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductFilterGroup}"/>.
        /// </returns>
        public IEnumerable<IProductFilterGroup> GetFilterGroupsContainingProduct(Guid productKey)
        {
            var cacheKey = string.Format("{0}.productfiltergroupcontaining", productKey);

            var filterGroup = (IEnumerable<IProductFilterGroup>)Cache.GetCacheItem(cacheKey);
            if (filterGroup != null) return filterGroup;

            return
                (IEnumerable<IProductFilterGroup>)
                Cache.GetCacheItem(
                    cacheKey,
                    () =>
                    Map(((EntityCollectionService)Service).GetEntityFilterGroupsContainingProduct(
                        _filterProviderKeys,
                            productKey)));
        }

        /// <summary>
        /// Gets a collection of <see cref="IProductFilterGroup"/> in which NONE of the filters contains a product with key passed as parameter.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductFilterGroup}"/>.
        /// </returns>
        public IEnumerable<IProductFilterGroup> GetFilterGroupsNotContainingProduct(Guid productKey)
        {
            var cacheKey = string.Format("{0}.productfiltergroupnotcontaining", productKey);

            var filterGroup = (IEnumerable<IProductFilterGroup>)Cache.GetCacheItem(cacheKey);
            if (filterGroup != null) return filterGroup;

            return
                (IEnumerable<IProductFilterGroup>)
                Cache.GetCacheItem(
                    cacheKey,
                    () =>
                    Map(((EntityCollectionService)Service).GetEntityFilterGroupsNotContainingProduct(
                        _filterProviderKeys,
                            productKey)));
        }

        /// <summary>
        /// Maps <see cref="IEntityFilterGroup"/> to <see cref="IProductFilterGroup"/>.
        /// </summary>
        /// <param name="group">
        /// The entity filter group.
        /// </param>
        /// <returns>
        /// The <see cref="IProductFilterGroup"/>.
        /// </returns>
        private static IProductFilterGroup Map(IEntityFilterGroup group)
        {
            return group.EntityTfKey == Constants.TypeFieldKeys.Entity.ProductKey && group.IsFilter ?
                new ProductFilterGroup(group) :
                null;
        }


        /// <summary>
        /// Maps a collection of <see cref="IEntityFilterGroup"/> to <see cref="IProductFilterGroup"/>.
        /// </summary>
        /// <param name="groups">
        /// The collections.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductFilterGroup}"/>.
        /// </returns>
        private static IEnumerable<IProductFilterGroup> Map(IEnumerable<IEntityFilterGroup> groups)
        {
            return groups.Select(Map).Where(x => x != null);
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        private void Initialize(EntityCollectionProviderResolver resolver)
        {
            _filterProviderKeys = resolver.GetProviderKeys<IEntityFilterGroupProvider>().ToArray();
        }
    }
}