namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.WebApi;
    using Merchello.Web.Workflow;

    using Umbraco.Web;
    using Umbraco.Web.Mvc;

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
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// The customer.
        /// </summary>
        private ICustomerBase _customer;

        /// <summary>
        /// The backoffice.
        /// </summary>
        private IBackoffice _backoffice;


        /// <summary>
        /// Initializes a new instance of the <see cref="OrderApiController"/> class.
        /// </summary>
        public OrderApiController()
            : this(Core.MerchelloContext.Current)
        {            
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="OrderApiController" /> class.
        /// </summary>
        /// <param name="merchelloContext">The merchello context.</param>        
        public OrderApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _orderService = merchelloContext.Services.OrderService;
            _invoiceService = merchelloContext.Services.InvoiceService;
            _merchello = new MerchelloHelper(merchelloContext.Services);
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
            : base(merchelloContext, umbracoContext)
        {
            _orderService = merchelloContext.Services.OrderService;
            _invoiceService = merchelloContext.Services.InvoiceService;
            _merchello = new MerchelloHelper(merchelloContext.Services);
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
        [HttpGet]
        public OrderDisplay GetOrder(Guid id)
        {
            return _merchello.Query.Order.GetByKey(id);
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
        [HttpGet]
        public IEnumerable<OrderLineItemDisplay> GetUnFulfilledItems(Guid id)
        {
            var order = _orderService.GetByKey(id);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found");
            }

            return order.UnfulfilledItems(MerchelloContext).Select(x => x.ToOrderLineItemDisplay());
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
            return _merchello.Query.Order.GetByInvoiceKey(id);
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
                throw new KeyNotFoundException("Invoice with id passed not found");
            }

            var shipmentLineItem = invoice.Items.FirstOrDefault(x => x.LineItemType == LineItemType.Shipping);

            if (shipmentLineItem == null)
            {
                throw new KeyNotFoundException("Shipment line item not found in the invoice");
            }

            var shipment = shipmentLineItem.ExtendedData.GetShipment<OrderLineItem>();
            if (shipment == null)
            {
                throw new KeyNotFoundException("Shipment not found in shipment line item extended data collection");
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
             
            _backoffice.Empty();
            _backoffice.Save();

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
        /// Adds items to the backoffice basket to calculate shipping and Sales tax
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AcceptVerbs("POST")]
        public IEnumerable<IShipmentRateQuote> GetShippingMethods(BackofficeAddItemModel model)
        {

            _customer = MerchelloContext.Services.CustomerService.GetAnyByKey(new Guid(model.CustomerKey));
            _backoffice = _customer.Backoffice();

            var shipment = _backoffice.PackageBackoffice(model.ShippingAddress.ToAddress()).FirstOrDefault();
            
            return shipment.ShipmentRateQuotes();
        }

        /// <summary>
        /// Adds items to the backoffice basket to calculate shipping and Sales tax
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        public IEnumerable<IPaymentGatewayMethod> GetPaymentMethods(BackofficeAddItemModel model)
        {
            return MerchelloContext.Gateways.Payment.GetPaymentGatewayMethods();
        }

        [HttpPost]
        public bool FinalizeBackofficeOrder(BackofficeAddItemModel model)
        {
            _customer = MerchelloContext.Services.CustomerService.GetAnyByKey(new Guid(model.CustomerKey));
            _backoffice = _customer.Backoffice();
                                                                    
            // This check asserts that we have enough
            // this should be handled a bit nicer for the customer.  
            if (!_backoffice.SalePreparation().IsReadyToInvoice()) return false;

            var preparation = _backoffice.SalePreparation();

            // Get the shipment again   
            var shippingAddress = _backoffice.SalePreparation().GetShipToAddress();

            var shipment = _backoffice.PackageBackoffice(shippingAddress).FirstOrDefault();

            // Clear any previously saved quotes (eg. the user went back to their basket and started the process over again).
            _backoffice.SalePreparation().ClearShipmentRateQuotes();

            // get the quote using the "approved shipping method"
            var quote = shipment.ShipmentRateQuoteByShipMethod(model.ShipmentKey);

            // save the quote                 
            _backoffice.SalePreparation().SaveShipmentRateQuote(quote);

            // for cash providers we only want to authorize the payment
            var paymentMethod = _backoffice.SalePreparation().GetPaymentMethod();

            IPaymentResult attempt;

            if (Merchello.Core.Constants.ProviderKeys.Payment.CashPaymentProviderKey == new Guid(model.PaymentProviderKey))
            {
                // AuthorizePayment will save the invoice with an Invoice Number.
                //
                attempt = preparation.AuthorizePayment(new Guid(model.PaymentKey));  
            }
            else // we 
            {
                // TODO wire in redirect to Credit Card view or PayPal ... etc.
                throw new NotImplementedException();
            }

            _backoffice.Empty();
            _backoffice.Save();

            return true;
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