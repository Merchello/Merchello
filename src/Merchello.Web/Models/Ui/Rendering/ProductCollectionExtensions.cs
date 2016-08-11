namespace Merchello.Web.Models.Ui.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Services;
    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// The product collection extensions.
    /// </summary>
    public static class ProductCollectionExtensions
    {
        /// <summary>
        /// Get the parent <see cref="ProductCollection"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="ProductCollection"/>.
        /// </returns>
        public static ProductCollection Parent(this ProductCollection value)
        {
            return value.ParentKey == null ? 
                       null : 
                       GetByKey(value.ParentKey.Value);
        }

        /// <summary>
        /// The child collection.
        /// </summary>
        /// <param name="value">
        /// The <see cref="ProductCollection"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ProductCollectionr}"/>.
        /// </returns>
        public static IEnumerable<ProductCollection> Children(this ProductCollection value)
        {
            return GetChildren(value);
        }

        /// <summary>
        /// Gets the collection of all products in the collection.
        /// </summary>
        /// <param name="value">
        /// The <see cref="ProductCollection"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        public static IEnumerable<IProductContent> GetProducts(this ProductCollection value)
        {
            return value.GetProducts(1, long.MaxValue);
        }

        /// <summary>
        /// Gets the collection of all products in the collection.
        /// </summary>
        /// <param name="value">
        /// The <see cref="ProductCollection"/>.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// Number of items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        public static IEnumerable<IProductContent> GetProducts(
            this ProductCollection value,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var merchelloHelper = new MerchelloHelper();
            return value.GetProducts(merchelloHelper, page, itemsPerPage, sortBy, sortDirection);
        }

        /// <summary>
        /// Gets a collection of <see cref="ProductCollection"/> that contain the product.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ProductCollection}"/>.
        /// </returns>
        public static IEnumerable<ProductCollection> Collections(this IProductContent product)
        {
            return product.Collections(MerchelloContext.Current);
        }

        /// <summary>
        /// Gets the collection of all products in the collection.
        /// </summary>
        /// <param name="value">
        /// The <see cref="ProductCollection"/>.
        /// </param>
        /// <param name="merchelloHelper">
        /// The <see cref="MerchelloHelper"/>.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// Number of items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        internal static IEnumerable<IProductContent> GetProducts(
            this ProductCollection value,
            MerchelloHelper merchelloHelper,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            return merchelloHelper.Query.Product.TypedProductContentFromCollection(
                value.CollectionKey,
                page,
                itemsPerPage,
                sortBy,
                sortDirection);
        }

        /// <summary>
        /// Returns a collection of ProductCollection for a given product.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ProductCollection}"/>.
        /// </returns>
        internal static IEnumerable<ProductCollection> Collections(this IProductContent product, IMerchelloContext merchelloContext)
        {
            var collections = GetProductEntityCollections(
                MerchelloContext.Current,
                product.Key,
                ((EntityCollectionService)merchelloContext.Services.EntityCollectionService).GetEntityCollectionsByProductKey);
                
                //((EntityCollectionService)merchelloContext.Services.EntityCollectionService).GetEntityCollectionsByProductKey(product.Key);

            return collections.Select(col => new ProductCollection(col));
        }

        /// <summary>
        /// Gets the child collections.
        /// </summary>
        /// <param name="value">
        /// The <see cref="ProductCollection"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ProductCollection}"/>.
        /// </returns>
        private static IEnumerable<ProductCollection> GetChildren(this ProductCollection value)
        {
            var service = MerchelloContext.Current.Services.EntityCollectionService;
            var children = GetCollectionChildCollections(MerchelloContext.Current, value.CollectionKey, ((EntityCollectionService)service).GetChildren);
            //var children = ((EntityCollectionService)service).GetChildren(value.CollectionKey);
            return children.Select(x => new ProductCollection(x));
        }

        /// <summary>
        /// Gets the <see cref="ProductCollection"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductCollection"/>.
        /// </returns>
        private static ProductCollection GetByKey(Guid key)
        {
            var service = MerchelloContext.Current.Services.EntityCollectionService;
            var collection = service.GetByKey(key);

            if (collection == null) return null;
            if (collection.EntityTfKey != Core.Constants.TypeFieldKeys.Entity.ProductKey) return null;

            return new ProductCollection(collection);
        }

        //// -- TODO these are quick request cache fixes that should be looked at as something more permanent
        //// in later version


        private static IEnumerable<IEntityCollection> GetProductEntityCollections(IMerchelloContext context, Guid productKey, Func<Guid, IEnumerable<IEntityCollection>> fetch)
        {
            var cacheKey = string.Format("{0}.productEntityCollections", productKey);

            var collections = (IEnumerable<IEntityCollection>)context.Cache.RequestCache.GetCacheItem(cacheKey);
            if (collections != null) return collections;

            return
                (IEnumerable<IEntityCollection>)
                context.Cache.RequestCache.GetCacheItem(cacheKey, () => fetch.Invoke(productKey));
        }

        private static IEnumerable<IEntityCollection> GetCollectionChildCollections(IMerchelloContext context, Guid collectionKey, Func<Guid, IEnumerable<IEntityCollection>> fetch)
        {
            var cacheKey = string.Format("{0}.entityCollectionChildCollection", collectionKey);

            var collections = (IEnumerable<IEntityCollection>)context.Cache.RequestCache.GetCacheItem(cacheKey);
            if (collections != null) return collections;

            return
                (IEnumerable<IEntityCollection>)
                context.Cache.RequestCache.GetCacheItem(cacheKey, () => fetch.Invoke(collectionKey));
        }
        
    }
}