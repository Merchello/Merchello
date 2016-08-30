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
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        public ProductFilterGroupService(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            this.Initialize();
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
            return group.EntityTfKey == Constants.TypeFieldKeys.Entity.ProductKey ?
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
            return groups.Where(x => x != null).Select(Map);
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        private void Initialize()
        {
            _filterProviderKeys = EntityCollectionProviderResolver.Current.GetProviderKeys<IEntityFilterGroupProvider>().ToArray();
        }
    }
}