using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Merchello.Core;
using Merchello.Core.Builders;
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
        /// Adds a shipment
        ///
        /// POST /umbraco/Merchello/ShipmentApi/CreateShipment
        /// </summary>
        /// <param name="order">POSTed <see cref="OrderDisplay"/> object</param>
        /// <remarks>
        /// 
        /// Note:  This is a modified order that very likely has not been persisted.  The UI 
        /// is responsible for removing line items that either have already shipped or are marked as back ordered.
        /// This "order" object should only contain line items intended to be included in the shipment to be created although
        /// other order data, such as the invoiceKey are important for this process.
        /// 
        /// </remarks>
        [AcceptVerbs("POST", "GET")]
        public ShipmentDisplay NewShipment(OrderDisplay order)
        {
            try
            {
                if(!order.Items.Any()) throw new InvalidOperationException("The shipment did not include any line items");
                
                var merchOrder = _orderService.GetByKey(order.Key);

                var builder = new ShipmentBuilderChain(MerchelloContext, order.ToOrder(merchOrder));

                var attempt = builder.Build();

                if(!attempt.Success)
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, attempt.Exception));

                return attempt.Result.ToShipmentDisplay();

            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message)));
            }
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