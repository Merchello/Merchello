using System;
using System.Linq;
using Examine;
using Examine.SearchCriteria;
using Merchello.Core;
using Merchello.Core.Models;
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


        private static void ReindexInvoice(IInvoice invoice)
        {
            ExamineManager.Instance.IndexProviderCollection[IndexName]
                .ReIndexNode(invoice.SerializeToXml().Root, IndexTypes.Invoice);
        }

    }
}