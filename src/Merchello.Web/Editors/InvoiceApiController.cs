namespace Merchello.Web.Editors
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.ContentEditing.Sales;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.WebApi;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// The invoice api controller.
    /// </summary>
    [PluginController("Merchello")]
    public class InvoiceApiController : MerchelloApiController
    {
        /// <summary>
        /// The store setting service.
        /// </summary>
        private readonly StoreSettingService _storeSettingService;

        /// <summary>
        /// The <see cref="IInvoiceService"/>.
        /// </summary>
        private readonly IInvoiceService _invoiceService;

        /// <summary>
        /// The note service.
        /// </summary>
        private readonly INoteService _noteService;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>
        /// </summary>
        private readonly MerchelloHelper _merchello;

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
            _storeSettingService = MerchelloContext.Services.StoreSettingService as StoreSettingService;
            _invoiceService = merchelloContext.Services.InvoiceService;
            _noteService = merchelloContext.Services.NoteService;
            _merchello = new MerchelloHelper(merchelloContext.Services, false);
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
        /// TODO rename to GetByKey
        public InvoiceDisplay GetInvoice(Guid id)
        {
            return _merchello.Query.Invoice.GetByKey(id);
        }

        /// <summary>
        /// Returns All Invoices
        /// 
        /// GET /umbraco/Merchello/InvoiceApi/SearchAllInvoices
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The paged collection of invoices.
        /// </returns>
        [HttpPost]
        public QueryResultDisplay SearchInvoices(QueryDisplay query)
        {
            var term = query.Parameters.FirstOrDefault(x => x.FieldName == "term");
            var invoiceDateStart = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateStart");
            var invoiceDateEnd = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateEnd");

            var isTermSearch = term != null && !string.IsNullOrEmpty(term.Value);

            var isDateSearch = invoiceDateStart != null && !string.IsNullOrEmpty(invoiceDateStart.Value);

            var startDate = DateTime.MinValue;
            var endDate = DateTime.MaxValue;
            if (isDateSearch)
            {
                var settings = _storeSettingService.GetAll().ToList();
                var dateFormat = settings.FirstOrDefault(s => s.Name == "dateFormat");

                if (dateFormat == null)
                {
                    if (!DateTime.TryParse(invoiceDateStart.Value, out startDate))
                    {
                        LogHelper.Warn<InvoiceApiController>(string.Format("Was unable to parse startDate: {0}", invoiceDateStart.Value));
                        startDate = DateTime.MinValue;
                    }
                    
                }
                else if (!DateTime.TryParseExact(invoiceDateStart.Value, dateFormat.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                {
                    LogHelper.Warn<InvoiceApiController>(string.Format("Was unable to parse startDate: {0}", invoiceDateStart.Value));
                    startDate = DateTime.MinValue;
                }

                endDate = invoiceDateEnd == null || dateFormat == null
                ? DateTime.MaxValue
                : DateTime.TryParseExact(invoiceDateEnd.Value, dateFormat.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate)
                    ? endDate
                    : DateTime.MaxValue;
            }
            

            if (isTermSearch && isDateSearch)
            {
                return _merchello.Query.Invoice.Search(
                    term.Value,
                    startDate,
                    endDate,
                    query.CurrentPage + 1,
                    query.ItemsPerPage,
                    query.SortBy,
                    query.SortDirection);
            }
            
            if (isTermSearch)
            {
                return this._merchello.Query.Invoice.Search(
                    term.Value,
                    query.CurrentPage + 1,
                    query.ItemsPerPage,
                    query.SortBy,
                    query.SortDirection);
            }
            
            if (isDateSearch)
            {
                return this._merchello.Query.Invoice.Search(
                    startDate,
                    endDate,
                    query.CurrentPage + 1,
                    query.ItemsPerPage,
                    query.SortBy,
                    query.SortDirection);
            }
            
            return this._merchello.Query.Invoice.Search(
                query.CurrentPage + 1,
                query.ItemsPerPage,
                query.SortBy,
                query.SortDirection);
        }    

        /// <summary>
        /// Gets a collection of invoices associated with a customer.
        /// 
        /// GET /umbraco/Merchello/InvoiceApi/SearchByCustomer/
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The collection of invoices associated with the customer.
        /// </returns>
        [HttpPost]
        public QueryResultDisplay SearchByCustomer(QueryDisplay query)
        {
            Guid key;
           
            var customerKey = query.Parameters.FirstOrDefault(x => x.FieldName == "customerKey");
            Mandate.ParameterNotNull(customerKey, "customerKey was null");
            Mandate.ParameterCondition(Guid.TryParse(customerKey.Value, out key), "customerKey was not a valid GUID");
           
            return _merchello.Query.Invoice.SearchByCustomer(
                key,
                query.CurrentPage + 1,
                query.ItemsPerPage,
                query.SortBy,
                query.SortDirection);
        }

        /// <summary>
        /// The search by date range.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        [HttpPost]
        public QueryResultDisplay SearchByDateRange(QueryDisplay query)
        {
            var invoiceDateStart = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateStart");
            var invoiceDateEnd = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateEnd");
            var invoiceStatusKey = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceStatusKey");

            DateTime startDate;
            DateTime endDate;
            Mandate.ParameterNotNull(invoiceDateStart, "invoiceDateStart is a required parameter");
            Mandate.ParameterCondition(DateTime.TryParse(invoiceDateStart.Value, out startDate), "Failed to convert invoiceDateStart to a valid DateTime");

            endDate = invoiceDateEnd == null
                ? DateTime.MaxValue
                : DateTime.TryParse(invoiceDateEnd.Value, out endDate)
                    ? endDate
                    : DateTime.MaxValue;

            return invoiceStatusKey == null
                ? _merchello.Query.Invoice.Search(
                    startDate,
                    endDate,
                    query.CurrentPage + 1,
                    query.ItemsPerPage,
                    query.SortBy,
                    query.SortDirection) :

                 _merchello.Query.Invoice.Search(
                    startDate,
                    endDate,
                    invoiceStatusKey.Value.EncodeAsGuid(),
                    query.CurrentPage + 1,
                    query.ItemsPerPage,
                    query.SortBy,
                    query.SortDirection);
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
        [HttpPost, HttpPut]
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
                LogHelper.Error<InvoiceApiController>("Failed to save invoice", ex);
                response = Request.CreateResponse(HttpStatusCode.NotFound, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Updates an invoice with adjustments.
        /// </summary>
        /// <param name="adjustments">
        /// The adjustments.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage PutInvoiceAdjustments(InvoiceAdjustmentDisplay adjustments)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var invoice = _invoiceService.GetByKey(adjustments.InvoiceKey);

                if (invoice != null)
                {
                    var invoiceItems = adjustments.Items.Select(x => x.ToInvoiceLineItem());

                    var success = ((InvoiceService)_invoiceService).AdjustInvoice(invoice, invoiceItems.ToArray());

                    if (success) return response;

                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Did not successfully adjust invoice");
                }

                response = Request.CreateResponse(HttpStatusCode.NotFound, "Invoice not found");
            }
            catch (Exception ex)
            {
                LogHelper.Error<InvoiceApiController>("Failed to adjust invoice", ex);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }
            

            return response;
        }

        /// <summary>
        /// The put invoice shipping address.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage PutInvoiceShippingAddress(InvoiceShippingUpdateData data)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var invoice = _invoiceService.GetByKey(data.InvoiceKey);
                var shippingLineItems = invoice.ShippingLineItems().ToArray();
                foreach (var lineItem in shippingLineItems)
                {
                    lineItem.ExtendedData.AddAddress(data.Address.ToAddress(), Constants.ExtendedDataKeys.ShippingDestinationAddress);
                }

                _invoiceService.Save(invoice);
            }
            catch (Exception ex)
            {
                LogHelper.Error<InvoiceApiController>("Failed to save shipping address", ex);
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
        [HttpPost, HttpDelete, HttpGet]
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