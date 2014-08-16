namespace Merchello.Web.Search
{
    using Core.Models;
    using Core.Services;
    using global::Examine;
    using global::Examine.Providers;

    using Merchello.Core;
    using Merchello.Core.Cache;
    using Merchello.Examine;

    using Models.ContentEditing;

    /// <summary>
    /// Responsible for invoice related queries - caches via lucene
    /// </summary>
    internal class CachedInvoiceQuery : CachedQueryBase<IInvoice, InvoiceDisplay>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedInvoiceQuery"/> class.
        /// </summary>
        public CachedInvoiceQuery() : this(
            MerchelloContext.Current.Services.InvoiceService,
            ExamineManager.Instance.IndexProviderCollection["MerchelloInvoiceIndexer"],
            ExamineManager.Instance.SearchProviderCollection["MerchelloInvoiceSearcher"])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedInvoiceQuery"/> class.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="indexProvider">
        /// The index provider.
        /// </param>
        /// <param name="searchProvider">
        /// The search provider.
        /// </param>
        internal CachedInvoiceQuery(
            IPageCachedService<IInvoice> service, 
            BaseIndexProvider indexProvider, 
            BaseSearchProvider searchProvider) 
            : base(service, indexProvider, searchProvider)
        {
        }

        /// <summary>
        /// Maps a <see cref="SearchResult"/> to <see cref="InvoiceDisplay"/>
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="InvoiceDisplay"/>.
        /// </returns>
        protected override InvoiceDisplay PerformMapSearchResultToDisplayObject(SearchResult result)
        {
            return result.ToInvoiceDisplay();
        }

        /// <summary>
        /// The re-index entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void ReindexEntity(IInvoice entity)
        {
            IndexProvider.ReIndexNode(entity.SerializeToXml().Root, IndexTypes.Invoice);
        }
    }
}