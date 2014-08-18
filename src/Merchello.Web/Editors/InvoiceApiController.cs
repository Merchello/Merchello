namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Search;
    using Merchello.Web.WebApi;

    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The invoice api controller.
    /// </summary>
    [PluginController("Merchello")]
    public class InvoiceApiController : MerchelloApiController
    {
        /// <summary>
        /// The _invoice service.
        /// </summary>
        private readonly IInvoiceService _invoiceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceApiController"/> class.
        /// </summary>
        public InvoiceApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public InvoiceApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _invoiceService = merchelloContext.Services.InvoiceService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        internal InvoiceApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _invoiceService = merchelloContext.Services.InvoiceService;
        }

        /// <summary>
        /// Returns an Invoice by id (key)
        /// 
        /// GET /umbraco/Merchello/InvoiceApi/GetInvoice/{guid}
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="InvoiceDisplay"/>.
        /// </returns>
        public InvoiceDisplay GetInvoice(Guid id)
        {
            var invoice = _invoiceService.GetByKey(id) as Invoice;
            if (invoice == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return invoice.ToInvoiceDisplay();
        }

        /// <summary>
        /// Returns All Invoices
        /// 
        /// GET /umbraco/Merchello/InvoiceApi/GetInvoices
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="InvoiceDisplay"/>.
        /// </returns>
        public QueryResultDisplay GetAllInvoices()
        {
            var page = ((InvoiceService)_invoiceService).GetPage(1, 100);

            return new QueryResultDisplay()
            {
                Items = page.Items.Select(InvoiceQuery.GetByKey),
                ItemsPerPage = page.ItemsPerPage,
                CurrentPage = page.CurrentPage,
                TotalPages = page.TotalPages,
                TotalItems = page.TotalItems
            };                        
        }

        /// <summary>
        /// Returns All Invoices
        /// 
        /// GET /umbraco/Merchello/InvoiceApi/GetAllInvoices
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The paged collection of invoices.
        /// </returns>
        [HttpPost]
        public QueryResultDisplay GetAllInvoices(QueryDisplay query)        
        {
            var page = ((InvoiceService)_invoiceService).GetPage(
                query.CurrentPage + 1,
                query.ItemsPerPage,
                query.SortBy,
                query.SortDirection);

            return new QueryResultDisplay()
            {
                Items = page.Items.Select(InvoiceQuery.GetByKey),
                ItemsPerPage = page.ItemsPerPage,
                CurrentPage = page.CurrentPage,
                TotalPages = page.TotalPages,
                TotalItems = page.TotalItems
            };            
        }

        /// <summary>
        /// Gets a collection of invoices associated with a customer.
        /// 
        /// GET /umbraco/Merchello/InvoiceApi/GetByCustomerKey/{id}
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The collection of invoices associated with the customer.
        /// </returns>
        public IEnumerable<InvoiceDisplay> GetByCustomerKey(Guid id)
        {
            return InvoiceQuery.GetByCustomerKey(id);
        }

        /// <summary>
        /// Returns All Invoices
        /// 
        /// GET /umbraco/Merchello/InvoiceApi/GetFilteredInvoices
        /// </summary>
        /// <param name="term">
        /// The search term
        /// </param>
        /// <returns>
        /// The collection of invoices..
        /// </returns>
        public QueryResultDisplay GetFilteredInvoices(string term)
        {
            var invoices = InvoiceQuery.Search(term).ToArray();

            return new QueryResultDisplay()
            {
                Items = invoices,
                CurrentPage = 0,
                TotalPages = 1,
                TotalItems = invoices.Count()
            };
        }

        /// <summary>
        /// Returns All Products
        /// 
        /// GET /umbraco/Merchello/InvoicesApi/GetFilteredInvoices
        /// </summary>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="perPage">
        /// The per Page.
        /// </param>
        /// <returns>
        /// The collection of invoices..
        /// </returns>
        public QueryResultDisplay GetFilteredInvoices(string term, int page, int perPage)
        {
            var allMatches = InvoiceQuery.Search(term).ToArray();
            var invoices = allMatches.Skip(page * perPage).Take(perPage);

            return new QueryResultDisplay()
            {
                Items = invoices,
                CurrentPage = page,
                TotalPages = ((allMatches.Count() - 1) / perPage) + 1,
                TotalItems = allMatches.Count()
            };
        }

        /// <summary>
        /// Updates an existing invoice
        /// 
        /// PUT /umbraco/Merchello/InvoiceApi/PutInvoice
        /// </summary>
        /// <param name="invoice">
        /// InvoiceDisplay object serialized from WebApi
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("POST", "PUT")]
        public HttpResponseMessage PutInvoice(InvoiceDisplay invoice)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var merchInvoice = _invoiceService.GetByKey(invoice.Key);
                merchInvoice = invoice.ToInvoice(merchInvoice);

                _invoiceService.Save(merchInvoice);
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Deletes an existing invoice
        /// 
        /// DELETE /umbraco/Merchello/InvoiceApi/{guid}
        /// </summary>
        /// <param name="id">
        /// The id of the invoice to delete
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("GET", "POST", "DELETE")]
        public HttpResponseMessage DeleteInvoice(Guid id)
        {
            var invoiceToDelete = _invoiceService.GetByKey(id);
            if (invoiceToDelete == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            _invoiceService.Delete(invoiceToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}