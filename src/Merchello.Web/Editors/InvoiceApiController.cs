namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Logging;
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
        /// The <see cref="IProductService"/>.
        /// </summary>
        private readonly IProductService _productService;

        /// <summary>
        /// The note service.
        /// </summary>
        private readonly INoteService _noteService;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// The <see cref="IOrderService"/>
        /// </summary>
        private readonly IOrderService _orderService;

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
            _productService = merchelloContext.Services.ProductService;
            _noteService = merchelloContext.Services.NoteService;
            _orderService = merchelloContext.Services.OrderService;
            _merchello = new MerchelloHelper(merchelloContext, false);
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

            // Get the invoice fresh to see if it solves back office problems
            //var invoice = _invoiceService.GetByKey(id);
            //var invoiceDisplay = AutoMapper.Mapper.Map<InvoiceDisplay>(invoice);
            //return invoiceDisplay;
        }

        /// <summary>
        /// Gets invoice item itemization.
        /// </summary>
        /// <param name="id">
        /// The invoice key
        /// </param>
        /// <returns>
        /// The <see cref="InvoiceItemItemizationDisplay"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if the invoice could not be retrieved by the Invoice service
        /// </exception>
        [HttpGet]
        public InvoiceItemItemizationDisplay GetInvoiceItemItemization(Guid id)
        {
            var invoice = _invoiceService.GetByKey(id);
            if (invoice == null) throw new NullReferenceException("Invoice not found");

            var itemization = invoice.ItemizeItems();
            return itemization.ToInvoiceItemItemizationDisplay();
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
                        MultiLogHelper.Warn<InvoiceApiController>(string.Format("Was unable to parse startDate: {0}", invoiceDateStart.Value));
                        startDate = DateTime.MinValue;
                    }

                }
                else if (!DateTime.TryParseExact(invoiceDateStart.Value, dateFormat.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                {
                    MultiLogHelper.Warn<InvoiceApiController>(string.Format("Was unable to parse startDate: {0}", invoiceDateStart.Value));
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
                MultiLogHelper.Error<InvoiceApiController>("Failed to save invoice", ex);
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
                MultiLogHelper.Error<InvoiceApiController>("Failed to adjust invoice", ex);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }


            return response;
        }

        ///// <summary>
        ///// Removes a product from an Invoice
        ///// </summary>
        ///// <param name="invoiceAddItems"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public HttpResponseMessage PutInvoiceRemoveProducts(InvoiceAddItems invoiceAddItems)
        //{
        //        	if (invoiceDisplay.SyncLineItems)
        //		    {
        //		        // See if any have been removed
        //                // Grab the current line items in a dictionary key'd by the SKU
        //		        var currentLineItems = destination.Items.ToDictionary(x => x.Sku, x => x);

        //              // Grab the proposed lines items
        //              var proposedLineItems = invoiceDisplay.Items.ToDictionary(x => x.Sku, x => x);

        //                // Loop the current line items and find the removed ones
        //		        foreach (var lineItem in currentLineItems)
        //		        {
        //		            if (!proposedLineItems.ContainsKey(lineItem.Value.Sku))
        //		            {
        //                        // This has been removed, so remove it from the destination items
        //		                  destination.Items.RemoveItem(lineItem.Value.Sku);
        //		            }
        //              }

        //                // Update quantities
        //		        foreach (var destinationItem in destination.Items)
        //		        {
        //		            if (proposedLineItems.ContainsKey(destinationItem.Sku))
        //		            {
        //		                var matching = proposedLineItems[destinationItem.Sku];
        //                      destinationItem.Quantity = matching.Quantity;
        //		            }
        //		        }

        //                // Update the currentlineitems dictionary now some have been removed
        //		          currentLineItems = destination.Items.ToDictionary(x => x.Sku, x => x);

        //                // Loops the proposed ones and find new ones to add and also update any quantities
        //                var lineItemsToAdd = new List<ILineItem>();
        //                foreach (var lineItemDisplay in proposedLineItems)
        //		        {
        //		            if (!currentLineItems.ContainsKey(lineItemDisplay.Value.Sku))
        //		            {
        //		                var lineItem = lineItemDisplay.Value;
        //                      lineItemsToAdd.Add(new InvoiceLineItem(lineItem.LineItemType, lineItem.Name, lineItem.Sku, lineItem.Quantity, lineItem.Price));
        //                    }
        //		        }

        //                // Add new line items if there are any
        //		        if (lineItemsToAdd.Any())
        //		        {
        //		            destination.Items.Add(lineItemsToAdd);
        //		        }
        //		    }
        //}

        /// <summary>
        /// Puts new products on an invoice
        /// </summary>
        /// <param name="invoiceAddItems"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage PutInvoiceNewProducts(InvoiceAddItems invoiceAddItems)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            //var currentUser = Umbraco.UmbracoContext.Security.CurrentUser;

            try
            {
                if (invoiceAddItems.Items != null)
                {
                    // Get the invoice
                    var merchInvoice = _invoiceService.GetByKey(invoiceAddItems.InvoiceKey);

                    // Check to see if we just have a SKU
                    foreach (var invoiceAddItem in invoiceAddItems.Items)
                    {
                        if (!string.IsNullOrEmpty(invoiceAddItem.Sku))
                        {
                            // Get the product/variant
                            var productBySku = _productService.GetBySku(invoiceAddItem.Sku);
                            var productVariantBySku = _productService.GetProductVariantBySku(invoiceAddItem.Sku);

                            // Update the data needed
                            invoiceAddItem.Key = productBySku != null ? productBySku.Key : productVariantBySku.Key;
                            invoiceAddItem.IsProductVariant = productBySku == null;
                        }
                    }

                    if (merchInvoice != null)
                    {
                        // Add the products
                        AddNewProductsToInvoice(merchInvoice, invoiceAddItems);
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.NotFound, "Invoice not found");
                    }

                }
            }
            catch (Exception ex)
            {
                MultiLogHelper.Error<InvoiceApiController>("Failed to adjust invoice", ex);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Create a InvoiceAddItem model
        /// </summary>
        /// <param name="merchInvoice"></param>
        /// <param name="product"></param>
        /// <param name="qty"></param>
        internal InvoiceAddItem ToInvoiceAddItem(IInvoice merchInvoice, IProduct product, int qty)
        {
            return new InvoiceAddItem { Key = product.Key, IsProductVariant = false, Quantity = qty };
        }

        /// <summary>
        /// Create a InvoiceAddItem model
        /// </summary>
        /// <param name="merchInvoice"></param>
        /// <param name="productVariant"></param>
        /// <param name="qty"></param>
        internal InvoiceAddItem ToInvoiceAddItem(IInvoice merchInvoice, IProductVariant productVariant, int qty)
        {
            return new InvoiceAddItem { Key = productVariant.Key, IsProductVariant = true, Quantity = qty };
        }

        /// <summary>
        /// Internal method to add products and orders to existing invoice
        /// // TODO - This is a bit chunky but it's because it's an after hack really
        /// // TODO - Also, it's a bit screwy as it seems back office is only ever setup for an invoice
        /// // TODO - to have a single order, BUT system is capable of multiple orders :/
        /// </summary>
        /// <param name="merchInvoice"></param>
        /// <param name="invoiceAddItems"></param>
        internal void AddNewProductsToInvoice(IInvoice merchInvoice, InvoiceAddItems invoiceAddItems)
        {
            // Get the current items in a dictionary so we can quickly check the SKU
            var currentLineItemsDict = merchInvoice.Items.ToDictionary(x => x.Sku, x => x);

            // Has orders
            var hasOrders = _orderService.GetOrdersByInvoiceKey(merchInvoice.Key).Any();

            // Store the orderlineitems
            var orderLineItemsToAdd = new List<OrderLineItem>();

            // Loop and add the new products as InvoiceLineItemDisplay to the InvoiceDisplay
            foreach (var invoiceAddItem in invoiceAddItems.Items)
            {
                // containers for the product or variant
                IProductVariant productVariant = null;
                IProduct product = null;

                // Get the variant or the product
                if (invoiceAddItem.IsProductVariant)
                {
                    productVariant = _productService.GetProductVariantByKey(invoiceAddItem.Key);
                }
                else
                {
                    product = _productService.GetByKey(invoiceAddItem.Key);
                }

                // If both null, just skip below
                if (productVariant == null && product == null) continue;

                // Get the sku to check
                var sku = product == null ? productVariant.Sku : product.Sku;

                // Create the lineitem
                var invoiceLineItem = product == null ? productVariant.ToInvoiceLineItem(invoiceAddItem.Quantity) : product.ToInvoiceLineItem(invoiceAddItem.Quantity);

                // Update Quantities
                foreach (var currentLineItem in merchInvoice.Items)
                {
                    if (currentLineItem.Sku == sku)
                    {
                        // We have a match!
                        currentLineItem.Quantity = (currentLineItem.Quantity + invoiceAddItem.Quantity);

                        if (hasOrders && currentLineItem.IsShippable())
                        {
                            // Add to Order   
                            orderLineItemsToAdd.Add(invoiceLineItem.AsLineItemOf<OrderLineItem>());

                            break;
                        }
                    }
                }

                // See if the current line items have this product/variant
                if (!currentLineItemsDict.ContainsKey(sku))
                {
                    merchInvoice.Items.Add(invoiceLineItem);

                    if (hasOrders && invoiceLineItem.IsShippable())
                    {
                        // Add to Order   
                        orderLineItemsToAdd.Add(invoiceLineItem.AsLineItemOf<OrderLineItem>());
                    }
                }
            }

            // Need to add the order
            if (hasOrders)
            {
                // Add to order or create a new one
                ((OrderService)_orderService).AddOrderLineItemsToEditedInvoice(orderLineItemsToAdd, merchInvoice);
            }

            // Now update invoice and save
            ((InvoiceService)_invoiceService).ReSyncInvoiceTotal(merchInvoice);
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
                MultiLogHelper.Error<InvoiceApiController>("Failed to save shipping address", ex);
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