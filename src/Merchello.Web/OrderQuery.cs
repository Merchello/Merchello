using System;
using System.Linq;
using Examine;
using Examine.SearchCriteria;
using Merchello.Core.Models;
using Merchello.Examine;
using Merchello.Web.Models.ContentEditing;

namespace Merchello.Web
{
    internal class OrderQuery : QueryBase
    {
        private const string IndexName = "MerchelloOrderIndexer";
        private const string SearcherName = "MerchelloOrderSearcher";

        /// <summary>
        /// Gets an <see cref="OrderDisplay"/> by it's unique key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A <see cref="OrderDisplay"/></returns>
        public static OrderDisplay GetByKey(Guid key)
        {
            return GetByKey(key.ToString());
        }

        /// <summary>
        /// Retrieves a <see cref="OrderDisplay"/> given it's 'unique' Key (string representation of the Guid)
        /// </summary>
        /// <param name="key"></param>
        /// <returns><see cref="OrderDisplay"/></returns>
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


        private static void ReindexOrder(IOrder order)
        {
            ExamineManager.Instance.IndexProviderCollection[IndexName]
                .ReIndexNode(order.SerializeToXml().Root, IndexTypes.Invoice);
        }
    }
}