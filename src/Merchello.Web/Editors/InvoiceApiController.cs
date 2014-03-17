using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Examine;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Web.Models.ContentEditing;
using Merchello.Web.WebApi;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class InvoiceApiController : MerchelloApiController
    {
        private readonly IInvoiceService _invoiceService;
       

        public InvoiceApiController()
            : this(MerchelloContext.Current)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public InvoiceApiController(IMerchelloContext merchelloContext)
            : base((MerchelloContext) merchelloContext)
        {
            _invoiceService = merchelloContext.Services.InvoiceService;
        }


        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal InvoiceApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base((MerchelloContext) merchelloContext, umbracoContext)
        {
            _invoiceService = merchelloContext.Services.InvoiceService;
        }

        /// <summary>
        /// Returns an Invoice by id (key)
        /// 
        /// GET /umbraco/Merchello/InvoiceApi/GetInvoice/{guid}
        /// </summary>
        /// <param name="id"></param>
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
        public IEnumerable<InvoiceDisplay> GetAllInvoices()
        {
            return InvoiceQuery.GetAllInvoices();
        }

        /// <summary>
        /// Returns All Products
        /// 
        /// GET /umbraco/Merchello/ProductApi/GetProducts
        /// </summary>
        public IEnumerable<InvoiceDisplay> GetAllInvoices(int page, int perPage)
        {
            return InvoiceQuery.GetAllInvoices().Skip((page - 1) * perPage).Take(perPage);
        }



        /// <summary>
        /// Returns All Invoices
        /// 
        /// GET /umbraco/Merchello/InvoiceApi/GetFilteredInvoices
        /// </summary>
        /// <param name="term"></param>
        public IEnumerable<InvoiceDisplay> GetFilteredInvoices(string term)
        {
            return InvoiceQuery.Search(term);
        }

        /// <summary>
        /// Returns All Products
        /// 
        /// GET /umbraco/Merchello/InvoicesApi/GetFilteredInvoices
        /// </summary>
        public IEnumerable<InvoiceDisplay> GetFilteredInvoices(string term, int page, int perPage)
        {
            return InvoiceQuery.Search(term).Skip((page - 1) * perPage).Take(perPage);
        }

        /// <summary>
        /// Updates an existing invoice
        ///
        /// PUT /umbraco/Merchello/InvoiceApi/PutInvoice
        /// </summary>
        /// <param name="invoice">InvoiceDisplay object serialized from WebApi</param>
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
                response = Request.CreateResponse(HttpStatusCode.NotFound, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Deletes an existing invoice
        ///
        /// DELETE /umbraco/Merchello/InvoiceApi/{guid}
        /// </summary>
        /// <param name="id"></param>
        [AcceptVerbs("POST", "DELETE")]
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