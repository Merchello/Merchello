namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Core.Configuration;
    using Core.Models.Rdbms;
    using Core.Persistence.Factories;
    using DataModifiers.Product;
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
    using Umbraco.Core.Models.Membership;
    using Umbraco.Core.Security;
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
        /// The <see cref="IShipmentService"/>
        /// </summary>
        private readonly IShipmentService _shipmentService;

        /// <summary>
        /// The <see cref="IAuditLogService"/>
        /// </summary>
        private readonly IAuditLogService _auditLogService;

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
            _shipmentService = merchelloContext.Services.ShipmentService;
            _auditLogService = merchelloContext.Services.AuditLogService;
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
            //return _merchello.Query.Invoice.GetByKey(id);

            // Get the invoice fresh to see if it solves back office problems
            // It's not returning orders so wondering if there is underlying cache issue here
            var invoice = _invoiceService.GetByKey(id);
            if (invoice != null)
            {
                // Always get orders directly as cannot find a consistant way to clear order cache on invoices
                var orders = _orderService.GetOrdersByInvoiceKey(id).ToArray();

                // They should match
                invoice.Orders = new OrderCollection();

                // Add the orders
                foreach (var order in orders)
                {
                    invoice.Orders.Add(order);
                }

                var invoiceDisplay = invoice.ToInvoiceDisplay();

                // TODO - Don't like this here. Need to speak to Rusty
                // Lastly, see if they have enabled 
                var canEnableInvoiceEdit = MerchelloConfiguration.Current.GetSetting("EnableInvoiceLineItemAddAndEditQty");
                if (!string.IsNullOrEmpty(canEnableInvoiceEdit) && Convert.ToBoolean(canEnableInvoiceEdit))
                {
                    // Enable edit
                    invoiceDisplay.EnableInvoiceEditQty = true;
                }

                return invoiceDisplay;
            }

            return null;
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

                    if (success)
                    {
                        if (CurrentUser != null)
                        {
                            // Set an audit log
                            var message = string.Format("Invoice {0} adjusted by {1}", invoice.InvoiceNumber, CurrentUser.Name);
                            _auditLogService.CreateAuditLogWithKey(invoice.Key, EntityType.Invoice, message);
                        }
                        return response;
                    }

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

        /// <summary>
        /// Cancels an invoice and the associated orders
        /// </summary>
        /// <param name="id">Invoice Id</param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost, HttpDelete, HttpGet]
        public HttpResponseMessage CancelInvoice(Guid id)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                // Get the invoice
                var invoice = _invoiceService.GetByKey(id);
                var invoiceNumber = invoice.InvoiceNumber;

                // Cancel all orders
                var orders = _orderService.GetOrdersByInvoiceKey(id);

                // TODO - This is a bit of a hack, must be a better way to create the order and invoice status??

                // Create a cancelled order status
                var cancelledOrderStatus = new OrderStatus
                {
                    Key = Constants.OrderStatus.Cancelled,
                    Alias = "cancelled",
                    Name = "Cancelled",
                    Active = true,
                    Reportable = false,
                    SortOrder = 1,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                };

                // Cancel each one
                foreach (var order in orders)
                {
                    // Set the order status
                    order.OrderStatus = cancelledOrderStatus;

                    // Save the order
                    _orderService.Save(order);
                }

                // Not sure if I need to do this??
                var cancelledInvoiceStatus = new InvoiceStatus
                {
                    Key = Constants.InvoiceStatus.Cancelled,
                    Alias = "cancelled",
                    Name = "Cancelled",
                    Active = true,
                    Reportable = false,
                    SortOrder = 1,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                };

                invoice.InvoiceStatus = cancelledInvoiceStatus;

                // Save the invoice
                _invoiceService.Save(invoice);

                if (CurrentUser != null)
                {
                    // Set an audit log
                    var message = string.Format("Invoice {0} cancelled by {1}", invoiceNumber, CurrentUser.Name);
                    _auditLogService.CreateAuditLogWithKey(invoice.Key, EntityType.Invoice, message);
                }
            }
            catch (Exception ex)
            {
                MultiLogHelper.Error<InvoiceApiController>("Failed to cancel invoice", ex);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        // TODO - Rename this method!
        /// <summary>
        /// Puts new products on an invoice
        /// </summary>
        /// <param name="invoiceAddItems"></param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public virtual HttpResponseMessage PutInvoiceNewProducts(InvoiceAddItems invoiceAddItems)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                if (invoiceAddItems.Items != null)
                {
                    // Get the invoice and associated data
                    var invoiceOrderShipment = _invoiceService.GetInvoiceOrderShipment(invoiceAddItems.InvoiceKey);

                    if (invoiceOrderShipment.Invoice != null)
                    {
                        //var currentUser = Umbraco.UmbracoContext.Security.CurrentUser;
                        var invoiceAdjustmentResult = new InvoiceAdjustmentResult(invoiceAddItems.LineItemType);

                        // The adjustment type
                        var invoiceAdjustmentType = SetInvoiceAdjustmentType(invoiceAddItems, invoiceOrderShipment);

                        //// Use the merchello helper to get the prdoucts, so the data modifiers are triggered.
                        var merchelloHelper = new MerchelloHelper();

                        // Get the product service
                        var productService = merchelloHelper.Query.Product;

                        // See if we can get the products
                        foreach (var invoiceAddItem in invoiceAddItems.Items)
                        {
                            if (invoiceAddItems.IsAddProduct && invoiceAddItem.Key != Guid.Empty)
                            {
                                invoiceAddItem.ProductVariant = productService.GetProductVariantByKey(invoiceAddItem.Key);
                                invoiceAddItem.Product = productService.GetByKey(invoiceAddItem.Key);

                                // Complete the Sku too
                                invoiceAddItem.Sku = invoiceAddItem.Product != null ? invoiceAddItem.Product.Sku : invoiceAddItem.ProductVariant.Sku;

                            }
                            else if (!string.IsNullOrWhiteSpace(invoiceAddItem.Sku))
                            {
                                invoiceAddItem.ProductVariant = productService.GetProductVariantBySku(invoiceAddItem.Sku);
                                invoiceAddItem.Product = productService.GetBySku(invoiceAddItem.Sku);
                            }
                        }

                        // Work out the type of adjustment
                        switch (invoiceAdjustmentType)
                        {
                            case InvoiceAdjustmentType.AddProducts:
                                invoiceAdjustmentResult = AddNewLineItemsToInvoice(invoiceOrderShipment, invoiceAddItems.Items, invoiceAdjustmentResult);
                                break;
                            case InvoiceAdjustmentType.DeleteProduct:
                                invoiceAdjustmentResult = DeleteLineItemsFromInvoice(invoiceOrderShipment, invoiceAddItems.Items, invoiceAdjustmentResult);
                                break;
                            case InvoiceAdjustmentType.UpdateProductDetails:
                                invoiceAdjustmentResult = UpdateCustomProductsOnInvoice(invoiceOrderShipment, invoiceAddItems.Items, invoiceAdjustmentResult);
                                break;
                        }

                        if (!invoiceAdjustmentResult.Success)
                        {
                            response = Request.CreateResponse(HttpStatusCode.Conflict, invoiceAdjustmentResult.Message);
                            MultiLogHelper.Warn<InvoiceApiController>(invoiceAdjustmentResult.Message);
                        }
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.NotFound, "Invoice not found");
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, "No items to update");
                }
            }
            catch (Exception ex)
            {
                MultiLogHelper.Error<InvoiceApiController>("Failed to adjust invoice", ex);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }


        /// <summary>
        /// Sets the adjustment type
        /// </summary>
        /// <param name="invoiceAddItems"></param>
        /// <param name="invoiceOrderShipment"></param>
        /// <returns></returns>
        public virtual InvoiceAdjustmentType SetInvoiceAdjustmentType(InvoiceAddItems invoiceAddItems, InvoiceOrderShipment invoiceOrderShipment)
        {
            if (invoiceOrderShipment.Invoice != null)
            {
                // If there is more than one item it's adding products
                if (invoiceAddItems.IsAddProduct)
                {
                    return InvoiceAdjustmentType.AddProducts;
                }

                // If there are any with 0 for qty it's a delete
                if (invoiceAddItems.Items.Any(x => x.Quantity <= 0))
                {
                    return InvoiceAdjustmentType.DeleteProduct;
                }

                // Lastly see if any updates
                if (invoiceAddItems.Items.Any(x => x.NeedsUpdating))
                {
                    return InvoiceAdjustmentType.UpdateProductDetails;
                }
            }

            // Default type which is ignored
            return InvoiceAdjustmentType.General;
        }

        /// <summary>
        /// Updates a line item details
        /// </summary>
        /// <param name="invoiceOrderShipment"></param>
        /// <param name="invoiceAddItems"></param>
        /// <param name="invoiceAdjustmentResult"></param>
        /// <returns></returns>
        public virtual InvoiceAdjustmentResult UpdateCustomProductsOnInvoice(InvoiceOrderShipment invoiceOrderShipment,
            IEnumerable<InvoiceAddItem> invoiceAddItems, InvoiceAdjustmentResult invoiceAdjustmentResult)
        {

            var addItems = invoiceAddItems.ToArray();

            // Get SKU's that need updating
            var skusToEdit = addItems.Where(x => x.NeedsUpdating).Select(x => x.OriginalSku);

            // Get the invoiceordershipmentlineitems
            var lineItems = invoiceOrderShipment.LineItems.Where(x => skusToEdit.Contains(x.Sku));

            // Loops the items to be updated
            foreach (var iosLineItem in lineItems)
            {
                if (iosLineItem.HasShipment)
                {
                    invoiceAdjustmentResult.Message = string.Format("Unable to update, as {0} is in a shipment.", iosLineItem.Name);
                    invoiceAdjustmentResult.Success = false;
                    return invoiceAdjustmentResult;
                }
            }

            foreach (var invoiceAddItem in addItems)
            {
                // If we get here we're good to update
                // Do the order
                foreach (var order in invoiceOrderShipment.Orders)
                {
                    foreach (var orderItem in order.Items)
                    {
                        if (orderItem.Sku == invoiceAddItem.OriginalSku)
                        {
                            orderItem.Sku = invoiceAddItem.Sku;
                            orderItem.Name = invoiceAddItem.Name;
                            orderItem.Quantity = invoiceAddItem.Quantity;
                            orderItem.Price = invoiceAddItem.Price;
                            if (orderItem.Price != invoiceAddItem.OriginalPrice)
                            {
                                orderItem.ExtendedData.SetValue("manuallyAdjustedPrice", "true");
                            }
                            break;
                        }
                    }
                }

                // Do the invoice
                foreach (var invoiceItem in invoiceOrderShipment.Invoice.Items)
                {
                    if (invoiceItem.Sku == invoiceAddItem.OriginalSku)
                    {
                        invoiceItem.Sku = invoiceAddItem.Sku;
                        invoiceItem.Name = invoiceAddItem.Name;
                        invoiceItem.Quantity = invoiceAddItem.Quantity;
                        invoiceItem.Price = invoiceAddItem.Price;
                        if(invoiceAddItem.Price != invoiceAddItem.OriginalPrice)
                        {
                            invoiceItem.ExtendedData.SetValue("manuallyAdjustedPrice", "true");
                        }
                        break;
                    }
                }
            }


            // If we get here we can update
            foreach (var order in invoiceOrderShipment.Orders)
            {
                _orderService.Save(order);
            }

            // Now update invoice and save as well as doing the tax
            ((InvoiceService)_invoiceService).ReSyncInvoiceTotal(invoiceOrderShipment.Invoice, true);

            // Set to true
            invoiceAdjustmentResult.Success = true;

            if (CurrentUser != null)
            {
                var message = string.Format("Updated lineitems on {0} by {1}", invoiceOrderShipment.Invoice.InvoiceNumber, CurrentUser.Name);
                _auditLogService.CreateAuditLogWithKey(invoiceOrderShipment.Invoice.Key, EntityType.Invoice, message);
            }

            return invoiceAdjustmentResult;
        }

        /// <summary>
        /// Delete products from an existing invoice
        /// </summary>
        /// <param name="invoiceOrderShipment"></param>
        /// <param name="invoiceAddItems"></param>
        /// <param name="invoiceAdjustmentResult"></param>
        public virtual InvoiceAdjustmentResult DeleteLineItemsFromInvoice(InvoiceOrderShipment invoiceOrderShipment, IEnumerable<InvoiceAddItem> invoiceAddItems, InvoiceAdjustmentResult invoiceAdjustmentResult)
        {
            // Get the items to be deleted in a dictionary by SKU too
            var skusToBeDeleted = invoiceAddItems.Where(x => x.Quantity <= 0).Select(x => x.OriginalSku);

            // Now get the correct items
            var toBeDeleted = invoiceOrderShipment.LineItems.Where(x => skusToBeDeleted.Contains(x.Sku)).ToArray();

            // Can we delete
            var canDelete = true;

            // Check if we have an order
            if (invoiceAdjustmentResult.InvoiceLineItemType == InvoiceLineItemType.Product)
            {
                // Order needs saving
                var saveOrder = false;

                foreach (var lineItem in toBeDeleted)
                {
                    if (lineItem.HasShipment)
                    {
                        // Cannot delete as this lineitem is in a shipment
                        canDelete = false;
                    }
                    else
                    {
                        if (invoiceOrderShipment.HasOrders)
                        {
                            var order = invoiceOrderShipment.Orders.FirstOrDefault(x => x.Key == lineItem.OrderId);
                            if (order != null)
                            {
                                // Remove this orderline item
                                invoiceOrderShipment.Orders.FirstOrDefault(x => x.Key == lineItem.OrderId).Items.RemoveItem(lineItem.Sku);

                                // Save the order
                                saveOrder = true;
                            }
                        }

                        // Remove invoice line item too
                        invoiceOrderShipment.Invoice.Items.RemoveItem(lineItem.Sku);

                    }

                    // Found a product we can't delete so break
                    if (canDelete == false)
                    {
                        break;
                    }

                    // Finally Save the order?
                    if (saveOrder)
                    {
                        foreach (var order in invoiceOrderShipment.Orders.Where(x => x.Key == lineItem.OrderId))
                        {
                            _orderService.Save(order);
                        }
                        saveOrder = false;
                    }
                }
            }

            // If we can delete then do it.
            if (canDelete)
            {
                // Now update invoice and save as well as doing the tax
                ((InvoiceService)_invoiceService).ReSyncInvoiceTotal(invoiceOrderShipment.Invoice, true);

                if (CurrentUser != null)
                {
                    var message = string.Format("Lineitems deleted from {0} by {1}", invoiceOrderShipment.Invoice.InvoiceNumber, CurrentUser.Name);
                    _auditLogService.CreateAuditLogWithKey(invoiceOrderShipment.Invoice.Key, EntityType.Invoice, message);
                }

                // Set to true
                invoiceAdjustmentResult.Success = true;
            }
            else
            {
                const string message = "Unable to delete product because there is already an order that has a shipment with one of the products to be deleted. Either delete the shipment or use adjustments to reduce invoice.";
                MultiLogHelper.Warn<InvoiceApiController>(message);
                invoiceAdjustmentResult.Success = false;
                invoiceAdjustmentResult.Message = message;
            }

            return invoiceAdjustmentResult;
        }

        /// <summary>
        /// Internal method to add products and orders to existing invoice
        /// </summary>
        /// <param name="invoiceOrderShipment"></param>
        /// <param name="invoiceAddItems"></param>
        /// <param name="invoiceAdjustmentResult"></param>
        public virtual InvoiceAdjustmentResult AddNewLineItemsToInvoice(InvoiceOrderShipment invoiceOrderShipment, IEnumerable<InvoiceAddItem> invoiceAddItems, InvoiceAdjustmentResult invoiceAdjustmentResult)
        {
            if (invoiceAdjustmentResult.InvoiceLineItemType == InvoiceLineItemType.Product)
            {
                // Get the current items in a dictionary so we can quickly check the SKU
                var currentLineItemsDict = invoiceOrderShipment.Invoice.Items.ToDictionary(x => x.Sku, x => x);

                // Store the orderlineitems
                var orderLineItems = new List<OrderLineItem>();

                // Loop and add the new products as InvoiceLineItemDisplay to the InvoiceDisplay
                foreach (var invoiceAddItem in invoiceAddItems)
                {
                    // If both null, just skip below
                    if (invoiceAddItem.ProductVariant == null && invoiceAddItem.Product == null) continue;

                    // Get the sku to check
                    var sku = invoiceAddItem.Product == null
                        ? invoiceAddItem.ProductVariant.Sku
                        : invoiceAddItem.Product.Sku;

                    // Product Pricing Enabled
                    var productPricingEnabled = MerchelloContext.Gateways.Taxation.ProductPricingEnabled;

                    // Create the lineitem
                    var invoiceLineItem = invoiceAddItem.Product == null
                        ? invoiceAddItem.ProductVariant.ToInvoiceLineItem(invoiceAddItem.Quantity, productPricingEnabled)
                        : invoiceAddItem.Product.ToInvoiceLineItem(invoiceAddItem.Quantity, productPricingEnabled);

                    // See if the current line items have this product/variant
                    if (!currentLineItemsDict.ContainsKey(sku))
                    {
                        invoiceOrderShipment.Invoice.Items.Add(invoiceLineItem);
                    }
                    else
                    {
                        // Already exists, just update qty
                        // Update Quantities
                        foreach (var currentLineItem in invoiceOrderShipment.Invoice.Items)
                        {
                            if (currentLineItem.Sku == sku)
                            {
                                // Update qty by one, as when adding they can only add one product at a time
                                currentLineItem.Quantity++;

                                // Break out of loop
                                break;
                            }
                        }
                    }

                    if (invoiceOrderShipment.HasOrders && invoiceLineItem.IsShippable())
                    {
                        // Add to Order   
                        orderLineItems.Add(invoiceLineItem.AsLineItemOf<OrderLineItem>());
                    }
                }

                // Need to add the order
                if (invoiceOrderShipment.HasOrders)
                {
                    // Add to order or create a new one
                    invoiceAdjustmentResult = ((OrderService)_orderService).AddOrderLineItemsToInvoice(orderLineItems, invoiceOrderShipment.Invoice, invoiceAdjustmentResult);
                    if (!invoiceAdjustmentResult.Success)
                    {
                        // Just return if there is an error, don't save anything
                        return invoiceAdjustmentResult;
                    }
                }

                // Now update invoice and save
                ((InvoiceService)_invoiceService).ReSyncInvoiceTotal(invoiceOrderShipment.Invoice, true);

                if (CurrentUser != null)
                {
                    var message = string.Format("Lineitems added to {0} by {1}", invoiceOrderShipment.Invoice.InvoiceNumber, CurrentUser.Name);
                    _auditLogService.CreateAuditLogWithKey(invoiceOrderShipment.Invoice.Key, EntityType.Invoice, message);
                }

                invoiceAdjustmentResult.Success = true;
            }
            else
            {
                invoiceAdjustmentResult.Success = false;
                invoiceAdjustmentResult.Message = "Only products can be added";
            }

            return invoiceAdjustmentResult;
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
                if (shippingLineItems.Any())
                {
                    foreach (var lineItem in shippingLineItems)
                    {
                        lineItem.ExtendedData.AddAddress(data.Address.ToAddress(), Constants.ExtendedDataKeys.ShippingDestinationAddress);
                    }
                }
                else
                {
                    // Problem. There is no shipping line item when there should be! Needs more investigating as this does very, very occasionally happen
                    MultiLogHelper.Warn<InvoiceApiController>(string.Format("No shipping address line item when trying to save shipping address for {0}", invoice.PrefixedInvoiceNumber()));
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

            if (CurrentUser != null)
            {
                var message = string.Format("Invoice {0} deleted by {1}", invoiceToDelete.InvoiceNumber, CurrentUser.Name);
                _auditLogService.CreateAuditLogWithKey(invoiceToDelete.Key, EntityType.Invoice, message);
            }

            _invoiceService.Delete(invoiceToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost, HttpDelete, HttpGet]
        public HttpResponseMessage DeleteDiscount(Guid invoiceId, string discountSku)
        {
            var invoice = _invoiceService.GetByKey(invoiceId);
            if (invoice == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            // Remove discount 
            ILineItem discountToDelete = null;
            foreach (var invoiceItem in invoice.Items)
            {
                if (invoiceItem.Sku == discountSku)
                {
                    discountToDelete = invoiceItem;
                    break;
                }
            }

            if (discountToDelete != null)
            {
                // Remove discount
                invoice.Items.Remove(discountToDelete);

                // Resync the invoice (And save)
                ((InvoiceService)_invoiceService).ReSyncInvoiceTotal(invoice, true);
            }


            if (CurrentUser != null)
            {
                var message = string.Format("Discount {0} deleted by {1}", invoice.InvoiceNumber, CurrentUser.Name);
                _auditLogService.CreateAuditLogWithKey(invoice.Key, EntityType.Invoice, message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}