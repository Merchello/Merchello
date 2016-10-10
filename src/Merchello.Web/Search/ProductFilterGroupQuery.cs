namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Merchello.Core;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Services;
    using Merchello.Web.Models;
    using Merchello.Web.Search.Provisional;

    using Umbraco.Core.Cache;

    /// <summary>
    /// Represents a ProductFilterGroupService.
    /// </summary>
    internal class ProductFilterGroupQuery : ProxyCollectionQueryBase, IProductFilterGroupQuery
    {

        /// <summary>
        /// Collection provider keys that designate a collection is a filter.
        /// </summary>
        private Guid[] _filterProviderKeys;


        private IPrimedProductFilterGroupTree _primedTree;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFilterGroupQuery"/> class.
        /// </summary>
        public ProductFilterGroupQuery()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFilterGroupQuery"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public ProductFilterGroupQuery(IMerchelloContext merchelloContext)
            : this(merchelloContext, EntityCollectionProviderResolver.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFilterGroupQuery"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello Context.
        /// </param>
        /// <param name="resolver">
        /// The resolver.
        /// </param>
        internal ProductFilterGroupQuery(IMerchelloContext merchelloContext, EntityCollectionProviderResolver resolver)
            : base(merchelloContext.Services.EntityCollectionService, merchelloContext.Cache.RequestCache)
        {
            Ensure.ParameterNotNull(merchelloContext, "merchelloContext");
            Ensure.ParameterNotNull(resolver, "resolver");
            this.Initialize(merchelloContext, resolver);
        }

        /// <summary>
        /// Gets a collection of provider responsible for managing entity collections that can be queries by this service.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IProviderInfo}"/>.
        /// </returns>
        public IEnumerable<IProviderMeta> GetCollectionProviders()
        {
            var atts = EntityCollectionProviderResolver.Current.GetProviderAttributes<Core.EntityCollections.IProductEntityFilterGroupProvider>();

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
            // This will be in the Runtime Cache from the underlying service
            return Map(((EntityCollectionService)this.Service).GetEntityFilterGroupByKey(key));
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
            var cacheKey = this.GetCacheKey("GetAll", keys);

            var filterGroups = (IEnumerable<IProductFilterGroup>)this.Cache.GetCacheItem(cacheKey);
            if (filterGroups != null) return filterGroups;

            var collections = ((EntityCollectionService)this.Service).GetEntityFilterGroupsByProviderKeys(this._filterProviderKeys);

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
            var cacheKey = this.GetCacheKey("GetFilterGroupsContainingProduct", productKey);

            var filterGroups = (IEnumerable<IProductFilterGroup>)this.Cache.GetCacheItem(cacheKey);
            if (filterGroups != null) return filterGroups;

            return
                (IEnumerable<IProductFilterGroup>)
                this.Cache.GetCacheItem(
                    cacheKey,
                    () =>
                    Map(((EntityCollectionService)this.Service).GetEntityFilterGroupsContainingProduct(
                        this._filterProviderKeys,
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
            var cacheKey = this.GetCacheKey("GetFilterGroupsNotContainingProduct", productKey);

            var filterGroup = (IEnumerable<IProductFilterGroup>)this.Cache.GetCacheItem(cacheKey);
            if (filterGroup != null) return filterGroup;

            return
                (IEnumerable<IProductFilterGroup>)
                this.Cache.GetCacheItem(
                    cacheKey,
                    () =>
                    Map(((EntityCollectionService)this.Service).GetEntityFilterGroupsNotContainingProduct(
                        this._filterProviderKeys,
                            productKey)));
        }

        /// <inheritdoc/>
        public IEnumerable<IPrimedProductFilterGroup> GetFilterGroupsForCollectionContext(params Guid[] collectionKeys)
        {
            var tree = _primedTree.GetTree(collectionKeys);

            return tree.Children.Select(x => x.Value.Item);
        }

        /// <summary>
        /// Clears the runtime cache for the filters.
        /// </summary>
        internal void ClearFilterTreeCache()
        {
            _primedTree.ClearCache();
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
        /// Used as a delegate for the tree.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IProductFilterGroup}"/>.
        /// </returns>
        private IEnumerable<IProductFilterGroup> All()
        {
            return GetAll();
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="resolver">
        /// The <see cref="IEntityCollectionProviderResolver"/>.
        /// </param>
        private void Initialize(IMerchelloContext merchelloContext, IEntityCollectionProviderResolver resolver)
        {
            this._filterProviderKeys = resolver.GetProviderKeys<IEntityFilterGroupProvider>().ToArray();
            this._primedTree = new PrimedProductFilterGroupTree(merchelloContext, this.All);
        }
    }
}