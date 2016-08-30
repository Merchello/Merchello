namespace Merchello.Web.Models.Ui.Rendering
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Services;

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
        public static IProductCollection Parent(this IProductCollection value)
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
        public static IEnumerable<IProductCollection> Children(this IProductCollection value)
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
        public static IEnumerable<IProductContent> GetProducts(this IProductCollection value)
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
            this IProductCollection value,
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
        public static IEnumerable<IProductCollection> Collections(this IProductContent product)
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
            this IProductCollection value,
            MerchelloHelper merchelloHelper,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            return merchelloHelper.Query.Product.TypedProductContentFromCollection(
                value.Key,
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
        internal static IEnumerable<IProductCollection> Collections(this IProductContent product, IMerchelloContext merchelloContext)
        {
            return GetProxyService(merchelloContext).GetCollectionsContainingProduct(product.Key);
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
        private static IEnumerable<IProductCollection> GetChildren(this IProductCollection value)
        {
            return GetProxyService(MerchelloContext.Current).GetChildCollections(value.Key);
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
        private static IProductCollection GetByKey(Guid key)
        {
            return GetProxyService(MerchelloContext.Current).GetByKey(key);
        }


        /// <summary>
        /// Gets the <see cref="IProductCollectionService"/>.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <returns>
        /// The <see cref="IProductCollectionService"/>.
        /// </returns>
        private static IProductCollectionService GetProxyService(IMerchelloContext merchelloContext)
        {
            return ProxyEntityServiceResolver.Current.Instance<ProductCollectionService>(new object[] { merchelloContext });
        }
        
    }
}