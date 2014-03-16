using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Examine.LuceneEngine.SearchCriteria;
using Examine.SearchCriteria;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Examine;
using Merchello.Web.Models.ContentEditing;

namespace Merchello.Web
{
    internal class InvoiceQuery : QueryBase
    {
        private const string IndexName = "MerchelloInvoiceIndexer";
        private const string SearcherName = "MerchelloInvoiceSearcher";

        /// <summary>
        /// Gets an <see cref="InvoiceDisplay"/> by it's unique key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A <see cref="InvoiceDisplay"/></returns>
        public static InvoiceDisplay GetByKey(Guid key)
        {
            return GetByKey(key.ToString());
        }

        /// <summary>
        /// Retrieves a <see cref="InvoiceDisplay"/> given it's 'unique' Key (string representation of the Guid)
        /// </summary>
        /// <param name="key"></param>
        /// <returns><see cref="InvoiceDisplay"/></returns>
        public static InvoiceDisplay GetByKey(string key)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria(BooleanOperation.And);
            criteria.Field("invoiceKey", key);

            var invoice = ExamineManager.Instance.SearchProviderCollection[SearcherName]
                .Search(criteria).Select(result => result.ToInvoiceDisplay()).FirstOrDefault();

            if (invoice != null) return invoice;
            var merchelloContext = GetMerchelloContext();

            var retrieved = merchelloContext.Services.InvoiceService.GetByKey(new Guid(key));
            
            if (retrieved == null) return null;

            ReindexInvoice(retrieved);

            return AutoMapper.Mapper.Map<InvoiceDisplay>(retrieved);
        }

        /// <summary>
        /// Gets a collection of all invoices
        /// </summary>
        public static IEnumerable<InvoiceDisplay> GetAllInvoices()
        {
            var merchelloContext = GetMerchelloContext();

            var criteria = ExamineManager.Instance.CreateSearchCriteria(IndexTypes.Invoice);
            criteria.Field("allDocs", "1");

            var results = ExamineManager.Instance.SearchProviderCollection[SearcherName]
                .Search(criteria).Select(result => result.ToInvoiceDisplay()).ToArray();


            var count = merchelloContext.Services.InvoiceService.InvoiceCount();

            if (results.Any() && (count == results.Count())) return results;

            if (count != results.Count())
            {
                RebuildIndex(IndexName);
            }

            var retrieved = ((InvoiceService)merchelloContext.Services.InvoiceService).GetAll();

            return retrieved.Select(AutoMapper.Mapper.Map<InvoiceDisplay>).ToList();
        }


        /// <summary>
        /// Searches InvoiceIndex by name and number for the 'term' passed
        /// </summary>
        /// <param name="term"></param>
        /// <returns>A collection of <see cref="InvoiceDisplay"/></returns>
        public static IEnumerable<InvoiceDisplay> Search(string term)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria();
            criteria.GroupedOr(new[] { "billToName", "invoiceNumber" }, term.Fuzzy());
            return Search(criteria);
        }

        /// <summary>
        /// Searches InvoiceIndex using <see cref="ISearchCriteria"/> passed
        /// </summary>
        /// <returns>A collection of <see cref="ProductDisplay"/></returns>
        public static IEnumerable<InvoiceDisplay> Search(ISearchCriteria criteria)
        {
            return ExamineManager.Instance.SearchProviderCollection[SearcherName]
                .Search(criteria).OrderByDescending(x => x.Score)
                .Select(result => result.ToInvoiceDisplay());
        }


        private static void ReindexInvoice(IInvoice invoice)
        {
            ExamineManager.Instance.IndexProviderCollection[IndexName]
                .ReIndexNode(invoice.SerializeToXml().Root, IndexTypes.Invoice);
        }

    }
}