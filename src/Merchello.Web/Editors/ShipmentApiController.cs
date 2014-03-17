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

        public ShipmentApiController()
            : this(MerchelloContext.Current)
        { }

        public ShipmentApiController(IMerchelloContext merchelloContext)
            : base((MerchelloContext) merchelloContext)
        {
            _shipmentService = merchelloContext.Services.ShipmentService;
            _invoiceService = merchelloContext.Services.InvoiceService;
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal ShipmentApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base((MerchelloContext) merchelloContext, umbracoContext)
        {
            _shipmentService = merchelloContext.Services.ShipmentService;
            _invoiceService = merchelloContext.Services.InvoiceService;
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

    }
}