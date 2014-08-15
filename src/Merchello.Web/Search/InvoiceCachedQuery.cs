namespace Merchello.Web.Search
{
    using Core.Models;
    using Core.Services;
    using global::Examine;
    using global::Examine.Providers;
    using Models.ContentEditing;

    internal class InvoiceCachedQuery : CachedQueryBase<IInvoice, InvoiceDisplay>
    {
        public InvoiceCachedQuery(IPageCachedService<IInvoice> service, BaseIndexProvider indexProvider, BaseSearchProvider searchProvider) 
            : base(service, indexProvider, searchProvider)
        {
        }

        protected override InvoiceDisplay PerformMapSearchResultToDisplayObject(SearchResult result)
        {
            return result.ToInvoiceDisplay();
        }

        protected override void ReindexEntity(IInvoice entity)
        {
            throw new System.NotImplementedException();
        }
    }
}