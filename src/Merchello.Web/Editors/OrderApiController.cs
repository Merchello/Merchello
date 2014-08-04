using System.Globalization;
using Merchello.Web.Workflow;

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

        private ICustomerBase _customer;

        private IBackoffice _backoffice;


        /// <summary>
        /// Initializes a new instance of the <see cref="OrderApiController"/> class.
        /// </summary>
        public OrderApiController()
            : this(MerchelloContext.Current)
        {            
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="OrderApiController" /> class.
        /// </summary>
        /// <param name="merchelloContext">The merchello context.</param>
        /// <param name="customer">The customer.</param>
        /// <param name="backoffice">The backoffice.</param>
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

            var shipment = shipmentLineItem.ExtendedData.GetShipment<OrderLineItem>();
            if (shipment == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return shipment.GetDestinationAddress().ToAddressDisplay();
        }

        /// <summary>
        /// Adds items to the backoffice basket to calculate shipping and Sales tax
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AcceptVerbs("POST")]
        public BackofficeOrderSummary ProcessesProductsToBackofficeOrder(BackofficeAddItemModel model)
        {
                                   
            _customer = MerchelloContext.Services.CustomerService.GetAnyByKey(new Guid(model.CustomerKey));
            _backoffice = _customer.Backoffice();

            if (model.ProductKeys != null && model.ProductKeys.Any())
            {

                foreach (var key in model.ProductKeys)
                {
                    var extendedData = new ExtendedDataCollection();
                    //extendedData.SetValue("umbracoContentId", model.ContentId.ToString(CultureInfo.InvariantCulture));

                    var product = MerchelloContext.Services.ProductService.GetByKey(new Guid(key));

                    //if (model.OptionChoices != null && model.OptionChoices.Any())
                    //{
                    //    var variant = MerchelloContext.Services.ProductVariantService.GetProductVariantWithAttributes(product, model.OptionChoices);

                    //    extendedData.SetValue("isVariant", "true");

                    //    _backoffice.AddItem(variant, variant.Name, 1, extendedData);
                    //}
                    //else
                    //{
                    _backoffice.AddItem(product, product.Name, 1, extendedData);
                    //}
                }

                var salesPreparation = _customer.Backoffice().SalePreparation();

                salesPreparation.SaveBillToAddress(model.BillingAddress.ToAddress());
                salesPreparation.SaveShipToAddress(model.ShippingAddress.ToAddress());
                
                return GetBackofficeOrderSummary(salesPreparation);
            }
            else
            {
                return new BackofficeOrderSummary();
            }
        }

        /// <summary>
        /// Gets the backoffice order summary.
        /// </summary>
        /// <param name="salesPreparation">The sales preparation.</param>
        /// <returns></returns>
        private static BackofficeOrderSummary GetBackofficeOrderSummary(BackofficeSalePreparation salesPreparation)
        {
            var summary = new BackofficeOrderSummary();

            if (!salesPreparation.IsReadyToInvoice()) return summary;

            var invoice = salesPreparation.PrepareInvoice();

            // item total
            summary.ItemTotal = invoice.TotalItemPrice();

            // shipping total
            summary.ShippingTotal = invoice.TotalShipping();

            // tax total
            summary.TaxTotal = invoice.TotalTax();

            // invoice total
            summary.InvoiceTotal = invoice.Total;

            return summary;
        }

        /// <summary>
        /// The get basket sale preparation.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The <see cref="BasketSalePreparation"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws if customer record cannot be found with key passed as argument
        /// </exception>
        private BasketSalePreparation GetSalePreparation(Guid customerKey)
        {
            // This is sort of weird to have the customer key in the ShippingAddress ... but we repurposed an object 
            // to simplify the JS
            var customer = MerchelloContext.Services.CustomerService.GetAnyByKey(customerKey);

            if (customer == null) throw new NullReferenceException(string.Format("The customer with key {0} was not found.", customerKey));

            return customer.Basket().SalePreparation();
        }
    }
}