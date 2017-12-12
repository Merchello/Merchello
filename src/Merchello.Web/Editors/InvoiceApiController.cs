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

                // Set an audit log
                var message = string.Format("Order cancelled by {0}", CurrentUser != null ? CurrentUser.Name : "Unable to get user");
                _auditLogService.CreateAuditLogWithKey(invoice.Key, EntityType.Invoice, message);
            }
            catch (Exception ex)
            {
                MultiLogHelper.Error<InvoiceApiController>("Failed to cancel invoice", ex);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Puts new products on an invoice
        /// </summary>
        /// <param name="invoiceAddItems"></param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage PutInvoiceNewProducts(InvoiceAddItems invoiceAddItems)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                if (invoiceAddItems.Items != null)
                {
                    // Get the invoice
                    var merchInvoice = _invoiceService.GetByKey(invoiceAddItems.InvoiceKey);

                    //// Use the merchello helper to get the prdoucts, so the data modifiers are triggered.
                    var merchelloHelper = new MerchelloHelper();

                    // Get the product service
                    var productService = merchelloHelper.Query.Product;

                    if (merchInvoice != null)
                    {
                        //var currentUser = Umbraco.UmbracoContext.Security.CurrentUser;
                        var invoiceAdjustmentResult = new InvoiceAdjustmentResult(invoiceAddItems.LineItemType);

                        // Check to see if we don't have a SKU
                        foreach (var invoiceAddItem in invoiceAddItems.Items)
                        {
                            if (string.IsNullOrEmpty(invoiceAddItem.Sku))
                            {
                                // Get the product/variant
                                invoiceAddItem.ProductVariant = productService.GetProductVariantByKey(invoiceAddItem.Key);
                                invoiceAddItem.Product = productService.GetByKey(invoiceAddItem.Key);
                                invoiceAddItem.Sku = invoiceAddItem.Product != null ? invoiceAddItem.Product.Sku : invoiceAddItem.ProductVariant.Sku;
                                invoiceAddItem.IsProductVariant = invoiceAddItem.Product == null;
                            }
                            else
                            {
                                invoiceAddItem.ProductVariant = productService.GetProductVariantBySku(invoiceAddItem.Sku);
                                invoiceAddItem.Product = productService.GetBySku(invoiceAddItem.Sku);
                                invoiceAddItem.IsProductVariant = invoiceAddItem.Product == null;
                            }
                        }

                        // If there is more than one item it's adding products
                        if (invoiceAddItems.IsAddProduct)
                        {
                            invoiceAdjustmentResult.InvoiceAdjustmentType = InvoiceAdjustmentType.AddProducts;
                        }
                        // If there are any with 0 for qty it's a delete
                        else if (invoiceAddItems.Items.Any(x => x.Quantity <= 0))
                        {
                            invoiceAdjustmentResult.InvoiceAdjustmentType = InvoiceAdjustmentType.DeleteProduct;
                        }
                        // If the new qty is greater than the original qty we are increasing
                        else if (invoiceAddItems.Items.Any(x => x.Quantity > x.OriginalQuantity))
                        {
                            invoiceAdjustmentResult.InvoiceAdjustmentType = InvoiceAdjustmentType.IncreaseProductQuantity;
                        }
                        // If the new qty is less, we are increasing
                        else if (invoiceAddItems.Items.Any(x => x.Quantity < x.OriginalQuantity))
                        {
                            invoiceAdjustmentResult.InvoiceAdjustmentType = InvoiceAdjustmentType.DecreaseProductQuantity;
                        }

                        // Work out the type of adjustment
                        switch (invoiceAdjustmentResult.InvoiceAdjustmentType)
                        {
                            case InvoiceAdjustmentType.AddProducts:
                                invoiceAdjustmentResult = AddNewLineItemsToInvoice(merchInvoice, invoiceAddItems.Items, invoiceAdjustmentResult);
                                break;
                            case InvoiceAdjustmentType.DecreaseProductQuantity:
                                invoiceAdjustmentResult = DecreaseLineItemQty(merchInvoice, invoiceAddItems.Items, invoiceAdjustmentResult);
                                break;
                            case InvoiceAdjustmentType.IncreaseProductQuantity:
                                invoiceAdjustmentResult = IncreaseLineItemQty(merchInvoice, invoiceAddItems.Items, invoiceAdjustmentResult);
                                break;
                            case InvoiceAdjustmentType.DeleteProduct:
                                invoiceAdjustmentResult = DeleteLineItemsFromInvoice(merchInvoice, invoiceAddItems.Items, invoiceAdjustmentResult);
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
            }
            catch (Exception ex)
            {
                MultiLogHelper.Error<InvoiceApiController>("Failed to adjust invoice", ex);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Delete products from an existing invoice
        /// </summary>
        /// <param name="merchInvoice"></param>
        /// <param name="invoiceAddItems"></param>
        /// <param name="invoiceAdjustmentResult"></param>
        internal InvoiceAdjustmentResult DeleteLineItemsFromInvoice(IInvoice merchInvoice, IEnumerable<InvoiceAddItem> invoiceAddItems, InvoiceAdjustmentResult invoiceAdjustmentResult)
        {
            // Get the current items in a dictionary so we can quickly check the SKU
            var currentLineItemsDict = merchInvoice.Items.ToDictionary(x => x.Sku, x => x);

            // Get the items to be deleted in a dictionary by SKU too
            var toBeDeleted = invoiceAddItems.Where(x => x.Quantity <= 0).ToDictionary(x => x.Sku, x => x);

            // Invoice orders
            var invoiceOrders = _orderService.GetOrdersByInvoiceKey(merchInvoice.Key).ToArray();

            // Can we delete
            var canDelete = true;

            // Check if we have an order
            if (invoiceOrders.Any() && invoiceAdjustmentResult.InvoiceLineItemType == InvoiceLineItemType.Product)
            {
                // Order needs saving
                var saveOrder = false;

                foreach (var invoiceAddItem in toBeDeleted)
                {
                    // We have orders, so we need to see if we can delete this product or it's too late
                    // Loop through the orders and see if this sku exists
                    // TODO - Bit shit, but we have to fish deep into this
                    foreach (var order in invoiceOrders)
                    {
                        var orderItems = order.Items;

                        // Loop items on the order
                        foreach (var orderItem in orderItems)
                        {
                            // See if one matches the product to be delete
                            if (orderItem.Sku == invoiceAddItem.Value.Sku)
                            {
                                // This order item has the product in

                                // Can we remove the order line item
                                var shipmentContainsSku = false;

                                // We have a match, see if this order has shipments   
                                var shipments = order.Shipments().ToArray();
                                if (shipments.Any())
                                {
                                    foreach (var shipment in shipments)
                                    {
                                        shipmentContainsSku = shipment.Items.Any(x => x.Sku == orderItem.Sku);
                                        if (shipmentContainsSku)
                                        {
                                            // Found so break
                                            break;
                                        }
                                    }
                                }

                                // Did we find a shipment with the Sku
                                if (shipmentContainsSku)
                                {
                                    // Cannot delete as found a shipment with the same sku
                                    canDelete = false;
                                }
                                else
                                {
                                    // Remove this orderline item
                                    order.Items.RemoveItem(orderItem.Sku);

                                    // Save the order
                                    saveOrder = true;

                                    // Get out of the loop and save
                                    break;
                                }
                            }

                            // Found a product so break
                            if (!canDelete)
                            {
                                break;
                            }
                        }

                        // Finally Save the order?
                        if (saveOrder)
                        {
                            _orderService.Save(order);
                            saveOrder = false;
                        }
                    }

                    // Found a product so break
                    if (!canDelete)
                    {
                        break;
                    }
                }
            }

            // If we can delete then do it.
            if (canDelete)
            {
                // No existing orders, remove from the invoice
                foreach (var merchInvoiceItem in currentLineItemsDict)
                {
                    if (toBeDeleted.ContainsKey(merchInvoiceItem.Value.Sku))
                    {
                        merchInvoice.Items.RemoveItem(merchInvoiceItem.Value.Sku);
                    }
                }

                // Now update invoice and save as well as doing the tax
                ((InvoiceService)_invoiceService).ReSyncInvoiceTotal(merchInvoice, true);

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
        /// <param name="merchInvoice"></param>
        /// <param name="invoiceAddItems"></param>
        /// <param name="invoiceAdjustmentResult"></param>
        internal InvoiceAdjustmentResult AddNewLineItemsToInvoice(IInvoice merchInvoice, IEnumerable<InvoiceAddItem> invoiceAddItems, InvoiceAdjustmentResult invoiceAdjustmentResult)
        {
            if (invoiceAdjustmentResult.InvoiceLineItemType == InvoiceLineItemType.Product)
            {
                //var taxationContext = MerchelloContext.Gateways.Taxation;

                var allOrders = _orderService.GetOrdersByInvoiceKey(merchInvoice.Key).ToArray();

                // Get the current items in a dictionary so we can quickly check the SKU
                var currentLineItemsDict = merchInvoice.Items.ToDictionary(x => x.Sku, x => x);

                // Has orders
                var hasOrders = allOrders.Any();

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
                        merchInvoice.Items.Add(invoiceLineItem);
                    }
                    else
                    {
                        // Already exists, just update qty
                        // Update Quantities
                        foreach (var currentLineItem in merchInvoice.Items)
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

                    if (hasOrders && invoiceLineItem.IsShippable())
                    {
                        // Add to Order   
                        orderLineItems.Add(invoiceLineItem.AsLineItemOf<OrderLineItem>());
                    }
                }

                // Need to add the order
                if (hasOrders)
                {
                    // Add to order or create a new one
                    invoiceAdjustmentResult = ((OrderService)_orderService).AddOrderLineItemsToInvoice(orderLineItems, merchInvoice, invoiceAdjustmentResult);
                    if (!invoiceAdjustmentResult.Success)
                    {
                        // Just return if there is an error, don't save anything
                        return invoiceAdjustmentResult;
                    }
                }

                // Now update invoice and save
                ((InvoiceService)_invoiceService).ReSyncInvoiceTotal(merchInvoice, true);

                invoiceAdjustmentResult.Success = true;
            }
            else
            {
                invoiceAdjustmentResult.Success = false;
                invoiceAdjustmentResult.Message = "Only products can be added";
            }

            return invoiceAdjustmentResult;
        }

        // TODO - Needs to be broken down. Too chunky
        /// <summary>
        /// Increases the line item qty of a product line item
        /// </summary>
        /// <param name="merchInvoice"></param>
        /// <param name="invoiceAddItems"></param>
        /// <param name="invoiceAdjustmentResult"></param>
        /// <returns></returns>
        internal InvoiceAdjustmentResult IncreaseLineItemQty(IInvoice merchInvoice, IEnumerable<InvoiceAddItem> invoiceAddItems, InvoiceAdjustmentResult invoiceAdjustmentResult)
        {
            // Change to array to stop multiple enumeration warning
            invoiceAddItems = invoiceAddItems.ToArray();

            // We are increasing the qty, so need to do a lot of checks.
            // Problems will be if there are shipments on the orders, and if any of those shipments have been 
            // shipped or more. We have to be sensible and just not allow it, if it is too complicated.
            // We start with lowest level, shipment, and abandon from there. As not point updating invoice if those

            // As we're check this way, we'll store everything that needs updating until we have checked from the bottom (shipments) 
            // to top (invoicelineitems) that everything is ok to continute
            var shipmentsToSave = new List<IShipment>();
            var ordersToSave = new List<IOrder>();

            // Get the orders for this invoice
            var allOrders = _orderService.GetOrdersByInvoiceKey(merchInvoice.Key).ToArray();
            if (allOrders.Any() && invoiceAdjustmentResult.InvoiceLineItemType == InvoiceLineItemType.Product)
            {
                // We should do some pre-checks
                var allShipments = allOrders.SelectMany(x => x.Shipments()).ToArray();

                // Can we update a shipment, if not we need to create a new order
                var updatedShipment = false;

                // Check each one passed, although it 'should' be only one
                foreach (var invoiceAddItem in invoiceAddItems)
                {
                    if (allShipments.Any())
                    {
                        // We have shipments. Loop through and see if we can match the items
                        foreach (var shipment in allShipments)
                        {
                            foreach (var shipmentItem in shipment.Items)
                            {
                                if (shipmentItem.Sku == invoiceAddItem.Sku)
                                {
                                    // Ooof. Found the item in a shipment.
                                    // Need to check if this shipment... Has well... Left the building.
                                    if (shipment.ShipmentStatusKey != Constants.ShipmentStatus.Shipped &&
                                        shipment.ShipmentStatusKey != Constants.ShipmentStatus.Delivered)
                                    {
                                        // Ooof. Again. Update qty and then save shipment
                                        updatedShipment = true;
                                        shipmentItem.Quantity = invoiceAddItem.Quantity;
                                        break;
                                    }
                                }
                            }

                            // Break shipment loop
                            if (updatedShipment)
                            {
                                // If we found it, add it to be saved
                                shipmentsToSave.Add(shipment);

                                break;
                            }
                        }
                    }

                    // Set this to true, as we'll create a new order unless we find one
                    var createNewOrder = true;


                    // If we didnt update a shipment, that means we either add it to an existing  order
                    // if there is one or we'll need to create a new one.
                    if (!updatedShipment)
                    {
                        // orders to check
                        var ordersToCheck = allOrders.Where(x => x.OrderStatusKey == Constants.OrderStatus.NotFulfilled).ToArray();

                        // Look through the nofulfilled orders (Ones that have no shipments)
                        if (ordersToCheck.Any())
                        {
                            // See if this item already exists and update quantity
                            foreach (var order in ordersToCheck)
                            {
                                // Find the order item with the same SKU
                                foreach (var orderLineItem in order.Items)
                                {
                                    // Found a match
                                    if (orderLineItem.Sku == invoiceAddItem.Sku)
                                    {
                                        // Update qty and then save the order
                                        createNewOrder = false;
                                        orderLineItem.Quantity = invoiceAddItem.Quantity;
                                        break;
                                    }
                                }

                                // Break shipment loop
                                if (!createNewOrder)
                                {
                                    // If we found it, add it to be saved
                                    ordersToSave.Add(order);

                                    break;
                                }
                            }

                            if (createNewOrder)
                            {
                                // Set flag to false as we are adding item to an order
                                createNewOrder = false;

                                // We have an order with no shipments, but this item isn't there
                                var lineItem = merchInvoice.Items.FirstOrDefault(x => x.Sku == invoiceAddItem.Sku);

                                // Should never be if we got here!!
                                if (lineItem != null)
                                {
                                    // Add the line item
                                    lineItem.Quantity = invoiceAddItem.Quantity;
                                    var order = ordersToCheck.FirstOrDefault();
                                    order.Items.Add(lineItem.AsLineItemOf<OrderLineItem>());
                                    ordersToSave.Add(order);
                                }
                            }
                        }

                    }
                    else
                    {
                        var orderFound = false;

                        // We have updated the shipment, so we can update the order items
                        // Loop all orders and just find the one that matches the SKU no matter what the order status is
                        foreach (var order in allOrders)
                        {
                            // Find the order item with the same SKU
                            foreach (var orderLineItem in order.Items)
                            {
                                // Found a match
                                if (orderLineItem.Sku == invoiceAddItem.Sku)
                                {
                                    // Update qty and then save the order
                                    orderFound = true;
                                    createNewOrder = false;
                                    orderLineItem.Quantity = invoiceAddItem.Quantity;
                                    break;
                                }
                            }

                            // Break shipment loop
                            if (orderFound)
                            {
                                // If we found it, add it to be saved
                                ordersToSave.Add(order);

                                break;
                            }
                        }
                    }

                    if (createNewOrder)
                    {
                        // If we get here it means there is either a shipment that's not active 
                        // or we could not find an order to add the lineitem too.
                        // We don't have an open order. So need to create a new one
                        var order = _orderService.CreateOrder(Constants.OrderStatus.NotFulfilled, merchInvoice.Key);
                        order.OrderNumberPrefix = merchInvoice.InvoiceNumberPrefix;

                        // find the line item with the same sku
                        var lineItem = merchInvoice.Items.FirstOrDefault(x => x.Sku == invoiceAddItem.Sku);

                        // Should never be if we got here!!
                        if (lineItem != null)
                        {
                            lineItem.Quantity = invoiceAddItem.Quantity;
                            order.Items.Add(lineItem.AsLineItemOf<OrderLineItem>());
                        }

                        // Add the new order to the invoice
                        ordersToSave.Add(order);
                    }
                }

                // Save shipments
                _shipmentService.Save(shipmentsToSave);

                // Save orders
                _orderService.Save(ordersToSave);
            }

            // Shipments done. ORders done. Now go through and update productlineitem on order
            // Loop and add the new products as InvoiceLineItemDisplay to the InvoiceDisplay
            foreach (var invoiceAddItem in invoiceAddItems)
            {
                // Update Quantities
                foreach (var currentLineItem in merchInvoice.Items)
                {
                    if (currentLineItem.Sku == invoiceAddItem.Sku)
                    {
                        // Update qty on invoice to the new quantity
                        currentLineItem.Quantity = invoiceAddItem.Quantity;

                        break;
                    }
                }
            }

            // Now update invoice and save inc re-adjusting tax
            ((InvoiceService)_invoiceService).ReSyncInvoiceTotal(merchInvoice, true);

            // Return
            invoiceAdjustmentResult.Success = true;

            return invoiceAdjustmentResult;
        }

        // TODO - Needs to be broken down. Too chunky
        /// <summary>
        /// Decreases the line item qty of a product line item and assocaited line items
        /// </summary>
        /// <param name="merchInvoice"></param>
        /// <param name="invoiceAddItems"></param>
        /// <param name="invoiceAdjustmentResult"></param>
        /// <returns></returns>
        internal InvoiceAdjustmentResult DecreaseLineItemQty(IInvoice merchInvoice, IEnumerable<InvoiceAddItem> invoiceAddItems, InvoiceAdjustmentResult invoiceAdjustmentResult)
        {
            // Change to array to stop multiple enumeration warning
            invoiceAddItems = invoiceAddItems.ToArray();

            // We are decreasing the qty, so need to do a lot of checks.
            // Firstly, we see if we can actually decrease the qty
            // Problems will be if there are shipments on the orders, and if any of those shipments have been 
            // shipped or more. OR if the shipments are broken into further shipments decreasing original shipment qty.
            // We have to be sensible and just not allow it, if it is too complicated.

            // We start with lowest level, shipment, and abandon from there. As not point updating invoice if those

            // As we're check this way, we'll store everything that needs updating until we have checked from the bottom (shipments) 
            // to top (invoicelineitems) that everything is ok to continute
            var shipmentsToSave = new List<IShipment>();
            var ordersToSave = new List<IOrder>();

            // Get the orders for this invoice
            var allOrders = _orderService.GetOrdersByInvoiceKey(merchInvoice.Key).ToArray();
            if (allOrders.Any())
            {
                // We should do some pre-checks
                var allShipments = allOrders.SelectMany(x => x.Shipments()).ToArray();

                if (allShipments.Any())
                {
                    // Check each one passed, although it 'should' be only one
                    foreach (var invoiceAddItem in invoiceAddItems)
                    {
                        var itemFound = false;
                        // We have shipments. Loop through and see if we can match the items to reduce
                        foreach (var shipment in allShipments)
                        {
                            foreach (var shipmentItem in shipment.Items)
                            {
                                if (shipmentItem.Sku == invoiceAddItem.Sku)
                                {
                                    // Ooof. Found the item in a shipment.
                                    // Need to check if this shipment... Has well... Left the building.
                                    if (shipment.ShipmentStatusKey != Constants.ShipmentStatus.Shipped &&
                                        shipment.ShipmentStatusKey != Constants.ShipmentStatus.Delivered)
                                    {
                                        // We can update... Now.. Do we have enough qty, we must have more 
                                        if (shipmentItem.Quantity >= invoiceAddItem.Quantity)
                                        {
                                            // Ooof. Again. Update qty and then save shipment
                                            itemFound = true;
                                            shipmentItem.Quantity = invoiceAddItem.Quantity;
                                            break;
                                        }

                                        // Abandon ship
                                        invoiceAdjustmentResult.Success = false;
                                        invoiceAdjustmentResult.Message = "Cannot reduce qty as the shipment qty does not match";
                                        return invoiceAdjustmentResult;
                                    }

                                    // Abandon ship
                                    invoiceAdjustmentResult.Success = false;
                                    invoiceAdjustmentResult.Message = "Cannot reduce qty as product has already shipped";
                                    return invoiceAdjustmentResult;
                                }
                            }

                            // Break shipment loop
                            if (itemFound)
                            {
                                // If we found it, add it to be saved
                                shipmentsToSave.Add(shipment);

                                break;
                            }
                        }
                    }

                }

                // Now go through the orders
                foreach (var invoiceAddItem in invoiceAddItems)
                {
                    var itemFound = false;

                    // Loop all orders
                    foreach (var order in allOrders)
                    {
                        // Find the order item with the same SKU
                        foreach (var orderLineItem in order.Items)
                        {
                            // Found a match
                            if (orderLineItem.Sku == invoiceAddItem.Sku)
                            {
                                // We can update... Now.. Do we have enough qty, we must have more 
                                if (orderLineItem.Quantity >= invoiceAddItem.Quantity)
                                {
                                    // Update qty and then save the order
                                    itemFound = true;
                                    orderLineItem.Quantity = invoiceAddItem.Quantity;
                                    break;
                                }

                                // Abandon ship
                                invoiceAdjustmentResult.Success = false;
                                invoiceAdjustmentResult.Message = "Cannot reduce qty as the order line item qty does not match";
                                return invoiceAdjustmentResult;
                            }
                        }

                        // Break shipment loop
                        if (itemFound)
                        {
                            // If we found it, add it to be saved
                            ordersToSave.Add(order);

                            break;
                        }
                    }
                }
            }

            // Shipments done. ORders done. Now go through and update productlineitem on order
            // Loop and add the new products as InvoiceLineItemDisplay to the InvoiceDisplay
            foreach (var invoiceAddItem in invoiceAddItems)
            {
                // Update Quantities
                foreach (var currentLineItem in merchInvoice.Items)
                {
                    if (currentLineItem.Sku == invoiceAddItem.Sku)
                    {
                        // Update qty on invoice to the new quantity
                        currentLineItem.Quantity = invoiceAddItem.Quantity;

                        break;
                    }
                }
            }

            // Save shipments
            foreach (var shipment in shipmentsToSave)
            {
                _shipmentService.Save(shipment);
            }

            // Save orders
            foreach (var order in ordersToSave)
            {
                _orderService.Save(order);
            }

            // Now update invoice and save inc re-adjusting tax
            ((InvoiceService)_invoiceService).ReSyncInvoiceTotal(merchInvoice, true);

            // Return
            invoiceAdjustmentResult.Success = true;

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