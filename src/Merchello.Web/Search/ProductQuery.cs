namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Examine;
    using global::Examine.LuceneEngine.SearchCriteria;
    using global::Examine.SearchCriteria;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Examine;
    using Merchello.Examine.Models;
    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The product query.
    /// </summary>
    [Obsolete("Use CachedProductQuery")]
    internal class ProductQuery : QueryBase
    {
        /// <summary>
        /// The index name.
        /// </summary>
        private const string IndexName = "MerchelloProductIndexer";

        /// <summary>
        /// The searcher name.
        /// </summary>
        private const string SearcherName = "MerchelloProductSearcher";

        /// <summary>
        /// Retrieves a <see cref="ProductDisplay"/> given it's 'unique' Key
        /// </summary>
        /// <param name="key">The product key</param>
        /// <returns><see cref="ProductDisplay"/></returns>
        [Obsolete("Use MerchelloHelper.Query.Product.GetByKey")]
        public static ProductDisplay GetByKey(Guid key)
        {
            return GetByKey(key.ToString());
        }

        /// <summary>
        /// Retrieves a <see cref="ProductDisplay"/> given it's 'unique' Key (string representation of the Guid)
        /// </summary>
        /// <param name="key">The product key</param>
        /// <returns><see cref="ProductDisplay"/></returns>
        [Obsolete("Use MerchelloHelper.Query.Product.GetByKey")]
        public static ProductDisplay GetByKey(string key)
        {
            var merchello = new MerchelloHelper(GetMerchelloContext().Services);
            return merchello.Query.Product.GetByKey(key.EncodeAsGuid());
        }

        /// <summary>
        /// Returns a collection of all products.
        /// </summary>
        /// <returns>
        /// The collection of all products.
        /// </returns>
        [Obsolete("Use CachedProductQuery", true)]
        public static IEnumerable<ProductDisplay> GetAllProducts()
        {

            var merchello = new MerchelloHelper(GetMerchelloContext().Services);

            return merchello.Query.Product.Search(0, int.MaxValue).Items.Select(x => (ProductDisplay)x);
        }

        /// <summary>
        /// Retrieves a <see cref="ProductVariantDisplay"/> given it's 'unique' key
        /// </summary>
        /// <param name="key">The product variant key</param>
        /// <returns>The <see cref="ProductVariantDisplay"/></returns>
        [Obsolete("Use MerchelloHelper.Query.Product.GetProductVariantByKey")]
        public static ProductVariantDisplay GetVariantDisplayByKey(Guid key)
        {
            return GetVariantDisplayByKey(key.ToString());
        }

        /// <summary>
        /// Retrieves a <see cref="ProductVariantDisplay"/> given it's 'unique' key (string representation of the Guid)
        /// </summary>
        /// <param name="key">The product variant key</param>
        /// <returns>The <see cref="ProductVariantDisplay"/></returns>
        [Obsolete("Use MerchelloHelper.Query.Product.GetProductVariantByKey")]
        public static ProductVariantDisplay GetVariantDisplayByKey(string key)
        {

            var merchello = new MerchelloHelper(GetMerchelloContext().Services);
            return null;
        }
    }
}