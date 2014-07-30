namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Core;
    using Core.Models;
    using Core.Services;
    using Models.ContentEditing;    
    using Umbraco.Web;
    using Umbraco.Web.Mvc;
    using WebApi;

    /// <summary>
    /// The order api controller.
    /// </summary>
    [PluginController("Merchello")]
    public class OrderApiController : MerchelloApiController
    {
        /// <summary>
        /// The order service.
        /// </summary>
        private readonly IOrderService _orderService;

        /// <summary>
        /// The invoice service.
        /// </summary>
        private readonly IInvoiceService _invoiceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderApiController"/> class.
        /// </summary>
        public OrderApiController()
            : this(MerchelloContext.Current)
        {            
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="OrderApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public OrderApiController(IMerchelloContext merchelloContext)
            : base((MerchelloContext) merchelloContext)
        {
            _orderService = merchelloContext.Services.OrderService;
            _invoiceService = merchelloContext.Services.InvoiceService;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="OrderApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        internal OrderApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base((MerchelloContext) merchelloContext, umbracoContext)
        {
            _orderService = merchelloContext.Services.OrderService;
            _invoiceService = merchelloContext.Services.InvoiceService;
        }

        /// <summary>
        /// Returns an Order by id (key)
        /// 
        /// GET /umbraco/Merchello/OrderApi/GetOrder/{guid}
        /// </summary>
        /// <param name="id">
        /// The order key
        /// </param>
        /// <returns>
        /// The <see cref="OrderDisplay"/>.
        /// </returns>
        public OrderDisplay GetOrder(Guid id)
        {
            var order = _orderService.GetByKey(id) as Order;

            if (order == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return order.ToOrderDisplay();
        }

        /// <summary>
        /// Returns an Orderby id (key) with line items flagged as Back Order if quantity not available in stock
        /// 
        /// GET /umbraco/Merchello/OrderApi/GetOrder/{guid}
        /// </summary>
        /// <param name="id">
        /// The order key
        /// </param>
        /// <remarks>
        /// 
        /// At this point we are not allowing for splitting of line items into mulitple shipments, but thats coming
        /// 
        /// </remarks>
        /// <returns>
        /// The collection of <see cref="OrderLineItemDisplay"/>.
        /// </returns>
        public IEnumerable<OrderLineItemDisplay> GetUnFulfilledItems(Guid id)
        {
            var order = _orderService.GetByKey(id);
            if (order == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return order.UnfulfilledItems().Select(x => x.ToOrderLineItemDisplay());
        }

        /// <summary>
        /// Returns an collection of orders for an invoice id (key)
        /// 
        /// GET /umbraco/Merchello/OrderApi/GetOrdersByInvoiceKey/{guid}
        /// </summary>
        /// <param name="id">
        /// The invoice key
        /// </param>
        /// <returns>
        /// The collection of <see cref="OrderDisplay"/>.
        /// </returns>
        public IEnumerable<OrderDisplay> GetOrdersByInvoiceKey(Guid id)
        {
            return OrderQuery.GetByInvoiceKey(id);
        }

        /// <summary>
        /// Returns the shipping Address by an invoice id (key)  - All orders for the invoice are assumed to be shipped to the same 
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="AddressDisplay"/>.
        /// </returns>
        public AddressDisplay GetShippingAddress(Guid id)
        {
            var invoice = _invoiceService.GetByKey(id);

            if (invoice == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            var shipmentLineItem = invoice.Items.FirstOrDefault(x => x.LineItemType == LineItemType.Shipping);

            if (shipmentLineItem == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            var shipment = shipmentLineItem.ExtendedData.GetShipment<InvoiceLineItem>();
            if (shipment == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return shipment.GetDestinationAddress().ToAddressDisplay();
        }
    }
}