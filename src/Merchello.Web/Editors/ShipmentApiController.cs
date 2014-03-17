using System;
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
    public class ShipmentApiController : MerchelloApiController
    {
        private readonly IShipmentService _shipmentService;
        private readonly IInvoiceService _invoiceService;
        private readonly IOrderService _orderService;

        public ShipmentApiController()
            : this(MerchelloContext.Current)
        { }

        public ShipmentApiController(IMerchelloContext merchelloContext)
            : base((MerchelloContext) merchelloContext)
        {
            _shipmentService = merchelloContext.Services.ShipmentService;
            _invoiceService = merchelloContext.Services.InvoiceService;
            _orderService = merchelloContext.Services.OrderService;
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal ShipmentApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base((MerchelloContext) merchelloContext, umbracoContext)
        {
            _shipmentService = merchelloContext.Services.ShipmentService;
            _invoiceService = merchelloContext.Services.InvoiceService;
            _orderService = merchelloContext.Services.OrderService;
        }

        /// <summary>
        /// Returns an shipment by id (key)
        /// 
        /// GET /umbraco/Merchello/ShipmentApi/GetShipment/{guid}
        /// </summary>
        /// <param name="id"></param>
        public ShipmentDisplay GetShipment(Guid id)
        {
            var shipment = _shipmentService.GetByKey(id) as Shipment;
            if (shipment == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return shipment.ToShipmentDisplay();
        }

        /// <summary>
        /// Returns an shipment based on a shipment rate quote stored in a shipping line item
        /// 
        /// GET /umbraco/Merchello/ShipmentApi/GetShipmentByInvoiceLineItem/?invoiceKey={guid}&lineItemKey={guid}
        /// </summary>
        [AcceptVerbs("GET", "POST")]
        public ShipmentDisplay GetShipmentByInvoiceLineItem(Guid invoiceKey, Guid lineItemKey)
        {
            // Get the invoice
            var invoice = _invoiceService.GetByKey(invoiceKey);
            if (invoice == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            // Get the line item from the invoice
            var shippingLineItem = invoice.Items.FirstOrDefault(x => x.LineItemType == LineItemType.Shipping && x.Key == lineItemKey);
            if (shippingLineItem == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            // Construct the shipment from values stored in ExtendedData
            var shipment = shippingLineItem.ExtendedData.GetShipment<InvoiceLineItem>();
            if (shipment == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return shipment.ToShipmentDisplay();
        }


        /// <summary>
        /// Adds a shipment
        ///
        /// POST /umbraco/Merchello/ShipmentApi/AddShipment
        /// </summary>
        /// <param name="shipment">POSTed <see cref="ShipmentDisplay"/> object</param>
        [AcceptVerbs("POST", "GET")]
        public HttpResponseMessage AddShipment(ShipmentDisplay shipment)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                if(!shipment.Items.Any()) throw new InvalidOperationException("The shipment did not include any line items");



            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Updates and existing shipment
        ///
        /// PUT /umbraco/Merchello/ShipmentApi/PutShipment
        /// </summary>
        /// <param name="shipment">ShipmentDisplay object serialized from WebApi</param>
        [AcceptVerbs("POST", "PUT")]
        public HttpResponseMessage PutShipment(ShipmentDisplay shipment)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var merchShipment = _shipmentService.GetByKey(shipment.Key);

                if (merchShipment == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                merchShipment = shipment.ToShipment(merchShipment);

                _shipmentService.Save(merchShipment);

            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, String.Format("{0}", ex.Message));
            }

            return response;
        }

        
    }
}