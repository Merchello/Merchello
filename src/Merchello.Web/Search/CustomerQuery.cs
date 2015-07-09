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
        [Obsolete("Use MerchelloHelper.Query.Customer.GetByKey")]
        public static CustomerDisplay GetByKey(string key)
        {
            var query = new CachedCustomerQuery(MerchelloContext.Current.Services.CustomerService, false);

            return query.GetByKey(new Guid(key));
        }

        /// <summary>
        /// The get all customers.
        /// </summary>
        /// <returns>
        /// The collection of all customers.
        /// </returns>
        [Obsolete("User MerchelloHelper.Query.Customer.Search")]
        public static IEnumerable<CustomerDisplay> GetAllCustomers()
        {
            var query = new CachedCustomerQuery(MerchelloContext.Current.Services.CustomerService, false);

            return query.Search(1, long.MaxValue).Items.Select(x => (CustomerDisplay)x);
        }

        ///// <summary>
        ///// Searches CustomerIndex by first name, last name, login name and email address for the 'term' passed
        ///// </summary>
        ///// <param name="term">Searches the customer index for a term</param>
        ///// <returns>A collection of <see cref="CustomerDisplay"/></returns>
        //public static IEnumerable<CustomerDisplay> Search(string term)
        //{
        //    var criteria = ExamineManager.Instance.CreateSearchCriteria();
        //    criteria.GroupedOr(
        //        new[] { "loginName", "firstName", "lastName", "email" },
        //        term.ToSearchTerms().Select(x => x.SearchTermType == SearchTermType.SingleWord ? x.Term.Fuzzy() : x.Term.MultipleCharacterWildcard()).ToArray());

        //    return Search(criteria);
        //}

        ///// <summary>
        ///// Searches CustomerIndex using <see cref="ISearchCriteria"/> passed
        ///// </summary>
        ///// <param name="criteria">
        ///// The criteria.
        ///// </param>
        ///// <returns>
        ///// A collection of <see cref="CustomerDisplay"/>
        ///// </returns>
        //public static IEnumerable<CustomerDisplay> Search(ISearchCriteria criteria)
        //{
        //    return ExamineManager.Instance.SearchProviderCollection[SearcherName]
        //        .Search(criteria).OrderByDescending(x => x.Score)
        //        .Select(result => result.ToCustomerDisplay());
        //}

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