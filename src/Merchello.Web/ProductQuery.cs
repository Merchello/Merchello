using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Examine.LuceneEngine.SearchCriteria;
using Examine.SearchCriteria;
using Merchello.Web.Models.ContentEditing;

namespace Merchello.Web
{
    public class ProductQuery
    {

        /// <summary>
        /// Retrieves a <see cref="ProductDisplay"/> given it's 'unique' Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns><see cref="ProductDisplay"/></returns>
        public static ProductDisplay GetByKey(Guid key)
        {
            return GetByKey(key.ToString());
        }

        /// <summary>
        /// Retrieves a <see cref="ProductDisplay"/> given it's 'unique' Key (string representation of the Guid)
        /// </summary>
        /// <param name="key"></param>
        /// <returns><see cref="ProductDisplay"/></returns>
        public static ProductDisplay GetByKey(string key)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria(BooleanOperation.And);
            criteria.Field("productKey", key).And().Field("master", "True");
            return ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"]
                .Search(criteria).Select(result => result.ToProductDisplay()).FirstOrDefault();
        }

        /// <summary>
        /// Searches ProductIndex by name and sku for the 'term' passed
        /// </summary>
        /// <param name="term"></param>
        /// <returns>A collection of <see cref="ProductDisplay"/></returns>
        public static IEnumerable<ProductDisplay> Search(string term)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria(BooleanOperation.Or);
            criteria.Field("name", term.Fuzzy(0.8f)).Or().Field("sku", term);
            return Search(criteria);
        }

        /// <summary>
        /// Searches ProductIndex using <see cref="ISearchCriteria"/> passed
        /// </summary>
        /// <returns>A collection of <see cref="ProductDisplay"/></returns>
        public static IEnumerable<ProductDisplay> Search(ISearchCriteria criteria)
        {
            return ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"]
                .Search(criteria).OrderByDescending(x => x.Score)
                .Select(result => result.ToProductDisplay());
        }
     
    }
}