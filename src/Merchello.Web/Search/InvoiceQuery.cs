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
    /// The invoice query.
    /// </summary>
    [Obsolete("Use CachedInvoiceQuery")]
    internal class InvoiceQuery : QueryBase
    {
        /// <summary>
        /// The Examine index name.
        /// </summary>
        private const string IndexName = "MerchelloInvoiceIndexer";

        /// <summary>
        /// The Examine searcher name.
        /// </summary>
        private const string SearcherName = "MerchelloInvoiceSearcher";

        /// <summary>
        /// The get by customer key.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// A collection of <see cref="InvoiceDisplay"/> associated with the customer.
        /// </returns>
        public static IEnumerable<InvoiceDisplay> GetByCustomerKey(Guid customerKey)
        {
            if (customerKey == Guid.Empty) return new List<InvoiceDisplay>();

            var criteria = ExamineManager.Instance.CreateSearchCriteria(IndexTypes.Invoice);
            criteria.Field("customerKey", customerKey.ToString());

            return ExamineManager.Instance.SearchProviderCollection[SearcherName].Search(criteria)
                    .Select(result => result.ToInvoiceDisplay(OrderQuery.GetByInvoiceKey))
                    .ToArray();
        }

        /// <summary>
        /// Searches InvoiceIndex by name and number for the 'term' passed
        /// </summary>
        /// <param name="term">Searches the invoice index for a term</param>
        /// <returns>A collection of <see cref="InvoiceDisplay"/></returns>
        public static IEnumerable<InvoiceDisplay> Search(string term)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria();
            criteria.Field("invoiceNumber", term).Or().GroupedOr(
                new[] { "billToName" },
                term.ToSearchTerms().Select(x => x.SearchTermType == SearchTermType.SingleWord ? x.Term.Fuzzy() : x.Term.MultipleCharacterWildcard()).ToArray());

            return Search(criteria);
        }

        /// <summary>
        /// Searches InvoiceIndex using <see cref="ISearchCriteria"/> passed
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// A collection of <see cref="InvoiceDisplay"/>
        /// </returns>
        public static IEnumerable<InvoiceDisplay> Search(ISearchCriteria criteria)
        {
            return ExamineManager.Instance.SearchProviderCollection[SearcherName]
                .Search(criteria).OrderByDescending(x => x.Score)
                .Select(result => result.ToInvoiceDisplay(OrderQuery.GetByInvoiceKey));
        }

        ///// <summary>
        ///// Gets a collection of all invoices
        ///// </summary>
        ///// <returns>
        ///// A collection of all <see cref="InvoiceDisplay"/>.
        ///// </returns>
        //internal static IEnumerable<InvoiceDisplay> GetAllInvoices()
        //{
        //    var merchelloContext = GetMerchelloContext();

        //    var criteria = ExamineManager.Instance.CreateSearchCriteria(IndexTypes.Invoice);
        //    criteria.Field("allDocs", "1");

        //    var results = ExamineManager.Instance.SearchProviderCollection[SearcherName]
        //        .Search(criteria).Select(result => result.ToInvoiceDisplay()).ToArray();


        //    var count = merchelloContext.Services.InvoiceService.CountInvoices();

        //    if (results.Any() && (count == results.Count())) return results;

        //    if (count != results.Count())
        //    {
        //        RebuildIndex(IndexName);
        //    }

        //    var retrieved = ((InvoiceService)merchelloContext.Services.InvoiceService).GetAll();

        //    return retrieved.Select(AutoMapper.Mapper.Map<InvoiceDisplay>).ToList();
        //}

        /// <summary>
        /// Gets an <see cref="InvoiceDisplay"/> by it's unique key
        /// </summary>
        /// <param name="key">The invoice key</param>
        /// <returns>A <see cref="InvoiceDisplay"/></returns>
        internal static InvoiceDisplay GetByKey(Guid key)
        {
            return GetByKey(key.ToString());
        }

        /// <summary>
        /// Retrieves a <see cref="InvoiceDisplay"/> given it's 'unique' Key (string representation of the Guid)
        /// </summary>
        /// <param name="key">The invoice key</param>
        /// <returns>The <see cref="InvoiceDisplay"/></returns>
        internal static InvoiceDisplay GetByKey(string key)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria();
            criteria.Field("invoiceKey", key);

            var invoice = ExamineManager.Instance.SearchProviderCollection[SearcherName]
                .Search(criteria).Select(result => result.ToInvoiceDisplay(OrderQuery.GetByInvoiceKey)).FirstOrDefault();

            if (invoice != null) return invoice;
            var merchelloContext = GetMerchelloContext();

            var retrieved = merchelloContext.Services.InvoiceService.GetByKey(new Guid(key));

            if (retrieved == null) return null;

            ReindexInvoice(retrieved);

            return AutoMapper.Mapper.Map<InvoiceDisplay>(retrieved);
        }
        
        /// <summary>
        /// ReIndexes an invoice.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        private static void ReindexInvoice(IInvoice invoice)
        {
            ExamineManager.Instance.IndexProviderCollection[IndexName]
                .ReIndexNode(invoice.SerializeToXml().Root, IndexTypes.Invoice);
        }
    }
}