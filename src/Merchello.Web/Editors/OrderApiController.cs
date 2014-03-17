using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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
    public class OrderApiController : MerchelloApiController
    {
        private readonly IOrderService _orderService;
        private readonly IInvoiceService _invoiceService;

        public OrderApiController()
            : this(MerchelloContext.Current)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public OrderApiController(IMerchelloContext merchelloContext)
            : base((MerchelloContext) merchelloContext)
        {
            _orderService = merchelloContext.Services.OrderService;
            _invoiceService = merchelloContext.Services.InvoiceService;
        }


        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
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
        /// <param name="id"></param>
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
        /// Returns an collection of orders for an invoice id (key)
        /// 
        /// GET /umbraco/Merchello/OrderApi/GetOrdersByInvoiceKey/{guid}
        /// </summary>
        /// <param name="id"></param>
        public IEnumerable<OrderDisplay> GetOrdersByInvoiceKey(Guid id)
        {
            return OrderQuery.GetByInvoiceKey(id);
        }

        /// <summary>
        /// Returns the shipping Address by an invoice id (key)  - All orders for the invoice are assumed to be shipped to the same 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AddressDisplay GetShippingAddress(Guid id)
        {
            var invoice = _invoiceService.GetByKey(id);
            if(invoice == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            var shipmentLineItem = invoice.Items.FirstOrDefault(x => x.LineItemType == LineItemType.Shipping);
            if(shipmentLineItem == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            var shipment = shipmentLineItem.ExtendedData.GetShipment<InvoiceLineItem>();
            if(shipment == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return shipment.GetDestinationAddress().ToAddressDisplay();
        }
    }
}