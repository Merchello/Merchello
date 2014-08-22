namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Examine;
    using global::Examine.LuceneEngine.SearchCriteria;
    using global::Examine.SearchCriteria;

    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Examine;
    using Merchello.Examine.Models;
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// The customer query.
    /// </summary>
    [Obsolete("Use CachedCustomerQuery")]
    internal class CustomerQuery : QueryBase
    {
        /// <summary>
        /// The index name.
        /// </summary>
        private const string IndexName = "MerchelloCustomerIndexer";

        /// <summary>
        /// The searcher name.
        /// </summary>
        private const string SearcherName = "MerchelloCustomerSearcher";

        /// <summary>
        /// Gets a customer by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>
        public static CustomerDisplay GetByKey(Guid key)
        {
            return GetByKey(key.ToString());
        }

        /// <summary>
        /// Gets a customer by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>
        public static CustomerDisplay GetByKey(string key)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria(BooleanOperation.And);
            criteria.Field("customerKey", key);

            var customer = ExamineManager.Instance.SearchProviderCollection[SearcherName]
                .Search(criteria).Select(result => result.ToCustomerDisplay()).FirstOrDefault();

            if (customer != null) return customer;
            var merchelloContext = GetMerchelloContext();

            var retrieved = merchelloContext.Services.CustomerService.GetByKey(new Guid(key));

            if (retrieved == null) return null;

            ReindexCustomer(retrieved);

            return AutoMapper.Mapper.Map<CustomerDisplay>(retrieved);
        }

        /// <summary>
        /// The get all customers.
        /// </summary>
        /// <returns>
        /// The collection of all customers.
        /// </returns>
        public static IEnumerable<CustomerDisplay> GetAllCustomers()
        {
            var merchelloContext = GetMerchelloContext();

            var criteria = ExamineManager.Instance.CreateSearchCriteria(IndexTypes.Customer);
            criteria.Field("allDocs", "1");

            var results = ExamineManager.Instance.SearchProviderCollection[SearcherName]
                .Search(criteria).Select(result => result.ToCustomerDisplay()).ToArray();


            var count = merchelloContext.Services.CustomerService.CustomerCount();

            if (results.Any() && (count == results.Count())) return results;

            if (count != results.Count())
            {
                RebuildIndex(IndexName);
            }

            var retrieved = ((CustomerService)merchelloContext.Services.CustomerService).GetAll();

            return retrieved.Select(AutoMapper.Mapper.Map<CustomerDisplay>).ToList();
        }

        /// <summary>
        /// Searches CustomerIndex by first name, last name, login name and email address for the 'term' passed
        /// </summary>
        /// <param name="term">Searches the customer index for a term</param>
        /// <returns>A collection of <see cref="CustomerDisplay"/></returns>
        public static IEnumerable<CustomerDisplay> Search(string term)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria();
            criteria.GroupedOr(
                new[] { "loginName", "firstName", "lastName", "email" },
                term.ToSearchTerms().Select(x => x.SearchTermType == SearchTermType.SingleWord ? x.Term.Fuzzy() : x.Term.MultipleCharacterWildcard()).ToArray());

            return Search(criteria);
        }

        /// <summary>
        /// Searches CustomerIndex using <see cref="ISearchCriteria"/> passed
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// A collection of <see cref="CustomerDisplay"/>
        /// </returns>
        public static IEnumerable<CustomerDisplay> Search(ISearchCriteria criteria)
        {
            return ExamineManager.Instance.SearchProviderCollection[SearcherName]
                .Search(criteria).OrderByDescending(x => x.Score)
                .Select(result => result.ToCustomerDisplay());
        }

        /// <summary>
        /// Re-indexes a customer.
        /// </summary>
        /// <param name="customer">
        /// The customer to re-index
        /// </param>
        private static void ReindexCustomer(ICustomer customer)
        {
            ExamineManager.Instance.IndexProviderCollection[IndexName]
                .ReIndexNode(customer.SerializeToXml().Root, IndexTypes.Customer);
        }
    }
}