using Merchello.Web.Models.SaleHistory;

namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Instrumentation;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Builders;
    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.WebApi;

    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The shipment api controller.
    /// </summary>
    [PluginController("Merchello")]
    public class ShipmentApiController : MerchelloApiController
    {
        /// <summary>
        /// The shipment service.
        /// </summary>
        private readonly IShipmentService _shipmentService;

        /// <summary>
        /// The invoice service.
        /// </summary>
        private readonly IInvoiceService _invoiceService;

        /// <summary>
        /// The order service.
        /// </summary>
        private readonly IOrderService _orderService;

        /// <summary>
        /// The ship method service.
        /// </summary>
        private readonly IShipMethodService _shipMethodService;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentApiController"/> class.
        /// </summary>
        public ShipmentApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public ShipmentApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _shipmentService = merchelloContext.Services.ShipmentService;
            _invoiceService = merchelloContext.Services.InvoiceService;
            _orderService = merchelloContext.Services.OrderService;
            _shipMethodService = ((ServiceContext)merchelloContext.Services).ShipMethodService;
            _merchello = new MerchelloHelper(merchelloContext.Services);
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello Context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco Context.
        /// </param>
        internal ShipmentApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _shipmentService = merchelloContext.Services.ShipmentService;
            _invoiceService = merchelloContext.Services.InvoiceService;
            _orderService = merchelloContext.Services.OrderService;
            _shipMethodService = ((ServiceContext)merchelloContext.Services).ShipMethodService;
            _merchello = new MerchelloHelper(merchelloContext.Services);
        }

        /// <summary>
        /// Returns an shipment by id (key)
        /// 
        /// GET /umbraco/Merchello/ShipmentApi/GetShipment/{guid}
        /// </summary>
        /// <param name="id">The shipment key</param>
        [HttpGet]
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
		/// Returns a list of shipments by their ids
		/// GET /umbraco/Merchello/ShipmentApi/GetShipments?ids={guid}&ids={guid}
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		[HttpGet]
        public IEnumerable<ShipmentDisplay> GetShipments([FromUri]IEnumerable<Guid> ids)
		{
            var keys = ids.Where(x => !x.Equals(Guid.Empty)).Distinct().ToArray();
		    
            if (!keys.Any()) return Enumerable.Empty<ShipmentDisplay>();
		    
            var shipments = _shipmentService.GetByKeys(keys);
		    if (shipments == null)
		    {
		        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
		    }

		    return shipments.Select(s => s.ToShipmentDisplay());
		}

        /// <summary>
        /// Gets the Shipmethod that was quoted for an order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        public ShipMethodDisplay GetShipMethod(OrderDisplay order)
        {
            var invoice = _invoiceService.GetByKey(order.InvoiceKey);

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

            if (shipment.ShipMethodKey != null)
            {
                var shipMethod = _shipMethodService.GetByKey(shipment.ShipMethodKey.Value);

                if (shipMethod == null) return new ShipMethodDisplay() { Name = "Not Found" };

                return shipMethod.ToShipMethodDisplay();
            }

            return new ShipMethodDisplay() { Name = "Not Found" };
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
        [HttpPost]
        public ShipmentDisplay NewShipment(OrderDisplay order)
        {
            try
            {
                if (!order.Items.Any()) throw new InvalidOperationException("The shipment did not include any line items");
                
                var merchOrder = _orderService.GetByKey(order.Key);

                var builder = new ShipmentBuilderChain(MerchelloContext, merchOrder, order.Items.Select(x => x.Key));

                var attempt = builder.Build();
                
                if (!attempt.Success)
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
        /// PUT /umbraco/Merchello/ShipmentApi/PutShipment
        /// </summary>
        /// <param name="shipment">ShipmentDisplay object serialized from WebApi</param>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        [HttpPost, HttpPut]
        public HttpResponseMessage PutShipment(ShipmentOrderDisplay shipmentOrder)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            var shipment = shipmentOrder.ShipmentDisplay;
            var order = shipmentOrder.OrderDisplay;
            try
            {
                var merchShipment = _shipmentService.GetByKey(shipment.Key);

                if (merchShipment == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                merchShipment = shipment.ToShipment(merchShipment);
                if (order.Items.Count() == shipment.Items.Count())
                {
                    merchShipment.AuditCreated();
                    Notification.Trigger("OrderShipped", merchShipment, new[] {merchShipment.Email});
                }
                else
                {
                    merchShipment.AuditCreated();            
                    Notification.Trigger("PartialOrderShipped", merchShipment, new[] { merchShipment.Email });
                }

                _shipmentService.Save(merchShipment);

            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Returns a list of all <see cref="IShipmentStatus"/>
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{ShipmentStatusDisplay}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<ShipmentStatusDisplay> GetAllShipmentStatuses()
        {
            var statuses = _shipmentService.GetAllShipmentStatuses().OrderBy(x => x.SortOrder);

            return statuses.Select(x => x.ToShipmentStatusDisplay());
        }
    }
}