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
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Models.Shipping;
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

		    return shipments.Select(s => s.ToShipmentDisplay()).OrderByDescending(x => x.ShipmentNumber);
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
        /// Gets the <see cref="ShipMethodDisplay"/> by it's key and alternative <see cref="ShipMethodDisplay"/> 
        /// for the same shipCountry.
        /// </summary>
        /// <param name="key">
        /// The shipmethod key from the quoted shipment 
        /// </param>
        /// <returns>
        /// The <see cref="ShipMethodsQueryDisplay"/>.
        /// </returns>
        [HttpGet]
        public ShipMethodsQueryDisplay GetShipMethodAndAlternatives(Guid key)
        {
            // Get the ShipMethod by the key
            var shipMethod = _shipMethodService.GetByKey(key);
            if (shipMethod == null) throw new NullReferenceException("Reference to ShipMethod passed was null. It must have been deleted.");

            // Get the ShipCountry
            var shipCountry = ((ServiceContext)MerchelloContext.Services).ShipCountryService.GetByKey(shipMethod.ShipCountryKey);
            if (shipCountry == null) throw new NullReferenceException("ShipCountry for ShipMethod passed was null. It must have been deleted.");

            // Get all providers associated with the ShipCountry
            var providers = MerchelloContext.Gateways.Shipping.GetGatewayProvidersByShipCountry(shipCountry);
            
            var allowed = new List<IShipMethod>();
            foreach (var shipMethods in providers.Select(provider => provider.GetAllShippingGatewayMethods(shipCountry)))
            {
                allowed.AddRange(shipMethods.Select(x => x.ShipMethod));
            }

            return new ShipMethodsQueryDisplay()
                {
                    Selected = shipMethod.ToShipMethodDisplay(),
                    Alternatives = allowed.Select(x => x.ToShipMethodDisplay())
                };
        }

        /// <summary>
        /// Adds a shipment
        ///
        /// POST /umbraco/Merchello/ShipmentApi/CreateShipment
        /// </summary>
        /// <param name="shipmentRequest">POSTed <see cref="ShipmentRequestDisplay"/> object</param>
        /// <remarks>
        /// 
        /// Note:  This is a modified order that very likely has not been persisted.  The UI 
        /// is responsible for removing line items that either have already shipped or are marked as back ordered.
        /// This "order" object should only contain line items intended to be included in the shipment to be created although
        /// other order data, such as the invoiceKey are important for this process.
        /// 
        /// </remarks>
        [HttpPost]
        public ShipmentDisplay NewShipment(ShipmentRequestDisplay shipmentRequest)
        {
            try
            {
                if (!shipmentRequest.Order.Items.Any()) throw new InvalidOperationException("The shipment did not include any line items");
                
                var merchOrder = _orderService.GetByKey(shipmentRequest.Order.Key);

                var builder = new ShipmentBuilderChain(MerchelloContext, merchOrder, shipmentRequest.Order.Items.Select(x => x.Key), shipmentRequest.ShipmentStatusKey, shipmentRequest.TrackingNumber);

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
        /// <returns>A <see cref="ShipmentDisplay"/></returns>
        [HttpPost, HttpPut]
        public ShipmentDisplay PutShipment(ShipmentDisplay shipment)
        {
            var merchShipment = _shipmentService.GetByKey(shipment.Key);
            var orderKeys = shipment.Items.Select(x => x.ContainerKey).Distinct();

            if (merchShipment == null) throw new NullReferenceException("Shipment not found for key");

            merchShipment = shipment.ToShipment(merchShipment);

            _shipmentService.Save(merchShipment);

            this.UpdateOrderStatus(orderKeys);

            return merchShipment.ToShipmentDisplay();
        }

        /// <summary>
        /// The update shipping address line item.
        /// </summary>
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage UpdateShippingAddressLineItem(ShipmentDisplay shipment)
        {
            var merchShipment = _shipmentService.GetByKey(shipment.Key);

            // get the order keys from the shipment.  In general this will be a single
            // order but it is possible for this to be an enumeration
            var orderKeys = shipment.Items.Select(x => x.ContainerKey).Distinct();
            var orders = _orderService.GetByKeys(orderKeys);

            // get the collection of invoices assoicated with the orders.
            // again, this is typically only one.
            var invoiceKeys = orders.Select(x => x.InvoiceKey).Distinct();
            var invoices = _invoiceService.GetByKeys(invoiceKeys);

            foreach (var invoice in invoices)
            {
                // now we're going to update every shipment line item with the destination address
                var shippingLineItems = invoice.ShippingLineItems().ToArray();
                foreach (var lineItem in shippingLineItems)
                {
                    lineItem.ExtendedData.AddAddress(merchShipment.GetDestinationAddress(), Constants.ExtendedDataKeys.ShippingDestinationAddress);
                }
                _invoiceService.Save(invoice);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Deletes an existing shipment
        /// 
        /// DELETE /umbraco/Merchello/ShipmentApi/{guid}
        /// </summary>
        /// <param name="id">
        /// The id of the shipment to delete
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost, HttpDelete, HttpGet]
        public HttpResponseMessage DeleteShipment(Guid id)
        {
            var shipmentToDelete = _shipmentService.GetByKey(id);
            
            if (shipmentToDelete == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var orderKeys = shipmentToDelete.Items.Select(x => x.ContainerKey).Distinct();
            
            _shipmentService.Delete(shipmentToDelete);

            this.UpdateOrderStatus(orderKeys);


            return Request.CreateResponse(HttpStatusCode.OK);
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

        private void UpdateOrderStatus(IEnumerable<Guid> orderKeys)
        {
            // update the order status
            var orders = _orderService.GetByKeys(orderKeys);
            foreach (var order in orders)
            {
                var orderStatusKey = order.ShippableItems().Any(x => ((OrderLineItem)x).ShipmentKey == null)
                                         ? Constants.DefaultKeys.OrderStatus.BackOrder
                                         : Constants.DefaultKeys.OrderStatus.Fulfilled;

                var orderStatus = _orderService.GetOrderStatusByKey(orderStatusKey);
                order.OrderStatus = orderStatus;
                _orderService.Save(order);
            }
        }
    }
}