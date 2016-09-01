namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Web.Models.Ui.Rendering;
    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Search;

    /// <summary>
    /// Extension methods for the <see cref="MerchelloHelper"/>
    /// </summary>
    public static class MerchelloHelperExtensions
    {
        #region QueryBuilders

        /// <summary>
        /// Gets the <see cref="IProductContentQueryBuilder"/>.
        /// </summary>
        /// <param name="merchello">
        /// The <see cref="MerchelloHelper"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContentQueryBuilder"/>.
        /// </returns>
        public static IProductContentQueryBuilder ProductQueryBuilder(this MerchelloHelper merchello)
        {
            return new ProductContentQueryBuilder(merchello.Query.Product);
        }
        

        #endregion

        #region Typed IProductContent Queries 

        /// <summary>
        /// Gets a <see cref="IProductContent"/> by it's key.
        /// </summary>
        /// <param name="merchello">
        /// The <see cref="MerchelloHelper"/>.
        /// </param>
        /// <param name="key">
        /// The key (id) of the product.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public static IProductContent TypedProductContent(this MerchelloHelper merchello, string key)
        {
            return merchello.Query.Product.TypedProductContent(new Guid(key));
        }

        /// <summary>
        /// Gets a <see cref="IProductContent"/> by it's key.
        /// </summary>
        /// <param name="merchello">
        /// The <see cref="MerchelloHelper"/>.
        /// </param>
        /// <param name="key">
        /// The key (id) of the product.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public static IProductContent TypedProductContent(this MerchelloHelper merchello, Guid key)
        {
            return merchello.Query.Product.TypedProductContent(key);
        }

        /// <summary>
        /// Gets a <see cref="IProductContent"/> by it's slug.
        /// </summary>
        /// <param name="merchello">
        /// The <see cref="MerchelloHelper"/>.
        /// </param>
        /// <param name="slug">
        /// The slug.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public static IProductContent TypedProductContentBySlug(this MerchelloHelper merchello, string slug)
        {
            return merchello.Query.Product.TypedProductContentBySlug(slug);
        }

        /// <summary>
        /// Gets a <see cref="IProductContent"/> by it's SKU.
        /// </summary>
        /// <param name="merchello">
        /// The <see cref="MerchelloHelper"/>.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public static IProductContent TypeProductContentBySku(this MerchelloHelper merchello, string sku)
        {
            return merchello.Query.Product.TypedProductContentBySku(sku);
        }

        /// <summary>
        /// Get the list of <see cref="IProductContent"/> by a collection of keys (ids).
        /// </summary>
        /// <param name="merchello">
        /// The <see cref="MerchelloHelper"/>.
        /// </param>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        public static IEnumerable<IProductContent> TypeProductContent(this MerchelloHelper merchello, IEnumerable<Guid> keys) // productKeys not productVariantKeys
        {
            return keys.Select(merchello.TypedProductContent);
        }

        #endregion

        #region Type IProductContent Collection Queries

        /// <summary>
        /// Get the list of <see cref="IProductContent"/> associated with the static product collection with key (id).
        /// </summary>
        /// <param name="merchello">
        /// The <see cref="MerchelloHelper"/>.
        /// </param>
        /// <param name="collectionKey">
        /// The key (id) of the collection.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        public static IEnumerable<IProductContent> TypedProductContentFromCollection(this MerchelloHelper merchello, Guid collectionKey)
        {
            return merchello.Query.Product.TypedProductContentFromCollection(collectionKey);
        }

        /// <summary>
        /// Get the list of <see cref="IProductContent"/> associated with the static product collection with key (id) for a give page of results.
        /// </summary>
        /// <param name="merchello">
        /// The <see cref="MerchelloHelper"/>.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="page">
        /// The current page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items Per Page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field (valid values are "sku", "name", "price").
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        public static IEnumerable<IProductContent> TypedProductContentFromCollection(this MerchelloHelper merchello, Guid collectionKey, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending)
        {
            if (page <= 0) page = 1;

            return merchello.Query.Product.TypedProductContentFromCollection(collectionKey, page, itemsPerPage, sortBy, sortDirection);
        }

        #endregion
    }
}