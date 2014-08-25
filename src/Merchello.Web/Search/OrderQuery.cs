namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Examine;
    using global::Examine.SearchCriteria;

    using Merchello.Core.Models;
    using Merchello.Examine;
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// The order query.
    /// </summary>
    [Obsolete("Use CachedOrderQuery")]
    internal class OrderQuery : QueryBase
    {
        /// <summary>
        /// The index name.
        /// </summary>
        private const string IndexName = "MerchelloOrderIndexer";

        /// <summary>
        /// The searcher name.
        /// </summary>
        private const string SearcherName = "MerchelloOrderSearcher";

        /// <summary>
        /// Gets an <see cref="OrderDisplay"/> by it's unique key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// A <see cref="OrderDisplay"/>
        /// </returns>
        public static OrderDisplay GetByKey(Guid key)
        {
            return GetByKey(key.ToString());
        }

        /// <summary>
        /// Retrieves a <see cref="OrderDisplay"/> given it's 'unique' Key (string representation of the Guid)
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// <see cref="OrderDisplay"/>
        /// </returns>
        public static OrderDisplay GetByKey(string key)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria(BooleanOperation.And);
            criteria.Field("orderKey", key);

            var order = ExamineManager.Instance.SearchProviderCollection[SearcherName]
                .Search(criteria).Select(result => result.ToOrderDisplay()).FirstOrDefault();

            if (order != null) return order;
            var merchelloContext = GetMerchelloContext();

            var retrieved = merchelloContext.Services.OrderService.GetByKey(new Guid(key));

            if (retrieved == null) return null;

            ReindexOrder(retrieved);

            return AutoMapper.Mapper.Map<OrderDisplay>(retrieved);
        }

        /// <summary>
        /// Gets a collection of orders for a given invoice
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice Key.
        /// </param>
        /// <returns>
        /// A collection of <see cref="OrderDisplay"/>
        /// </returns>
        public static IEnumerable<OrderDisplay> GetByInvoiceKey(Guid invoiceKey)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria();
            criteria.Field("invoiceKey", invoiceKey.ToString());

            return ExamineManager.Instance.SearchProviderCollection[SearcherName]
                .Search(criteria).Select(result => result.ToOrderDisplay());

        }

        /// <summary>
        /// The re-index order.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        private static void ReindexOrder(IOrder order)
        {
            ExamineManager.Instance.IndexProviderCollection[IndexName]
                .ReIndexNode(order.SerializeToXml().Root, IndexTypes.Invoice);
        }
    }
}