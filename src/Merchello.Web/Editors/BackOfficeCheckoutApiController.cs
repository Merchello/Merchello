namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.ContentEditing.Checkout;
    using Merchello.Web.Models.Payments;
    using Merchello.Web.WebApi;
    using Merchello.Web.Workflow;
    using Merchello.Web.Workflow.CustomerItemCache;

    using Umbraco.Core.Logging;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// A controller for back office checkouts.
    /// </summary>
    [PluginController("Merchello")]
    public class BackOfficeCheckoutApiController : MerchelloApiController
    {
        #region Fields

        /// <summary>
        /// The <see cref="MerchelloHelper"/>
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// The customer service.
        /// </summary>
        private readonly ICustomerService _customerService;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BackOfficeCheckoutApiController"/> class.
        /// </summary>
        public BackOfficeCheckoutApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackOfficeCheckoutApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public BackOfficeCheckoutApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _customerService = merchelloContext.Services.CustomerService;
            _merchello = new MerchelloHelper(merchelloContext.Services);
        }

        #region Payment Methods

        /// <summary>
        /// The get payment methods.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{PaymentMethodDisplay}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<PaymentMethodDisplay> GetPaymentMethods()
        {
            var paymentMethods = MerchelloContext.Gateways.Payment.GetPaymentGatewayMethods();

            return paymentMethods.Select(x => x.ToPaymentMethodDisplay());
        }

        /// <summary>
        /// Gets a collection of shipment rate quotes for a customer's basket.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ShipmentRateQuoteDisplay}"/>.
        /// </returns>
        /// <remarks>
        /// Assumes the shipping address is the default shipping address
        /// </remarks>
        [HttpGet]
        public IEnumerable<ShipmentRateQuoteDisplay> GetShipmentRateQuotes(Guid customerKey)
        {
            // Gets the customer
            var customer = _customerService.GetByKey(customerKey);
            if (customer == null) return Enumerable.Empty<ShipmentRateQuoteDisplay>();

            // Gets the default shipping address saved with the customer
            var shippingAddress = customer.DefaultCustomerAddress(AddressType.Shipping);
            if (shippingAddress == null) return Enumerable.Empty<ShipmentRateQuoteDisplay>();

            // Gets the customer basket
            var basket = customer.Basket();
            if (basket.IsEmpty) return Enumerable.Empty<ShipmentRateQuoteDisplay>();

            var shipments = basket.PackageBasket(shippingAddress.AsAddress(shippingAddress.FullName));

            // Quotes each shipment
            var rateQuotes = new List<ShipmentRateQuoteDisplay>();
            foreach (var shipment in shipments.ToArray())
            {
                rateQuotes.AddRange(
                    shipment
                    .ShipmentRateQuotes()
                    .Select(x => x.ToShipmentRateQuoteDisplay())
                    .Where(x => x != null));
            }

            return rateQuotes;
        }

        #endregion

        #region Basket / Wishlist

        /// <summary>
        /// Adds an item to the customer item cache.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ItemCacheLineItemDisplay}"/>.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage AddItemCacheItem(AddToItemCacheInstruction instruction)
        {
            var itemCache = GetCustomerItemCache(instruction);

            if (itemCache == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var variants = new List<ProductVariantDisplay>();
            foreach (var addItem in instruction.Items.ToArray())
            {
                if (addItem.IsProductVariant)
                {
                    var variant = _merchello.Query.Product.GetProductVariantByKey(addItem.Key);
                    if (variant != null) variants.Add(variant);
                }
                else
                {
                    var product = _merchello.Query.Product.GetByKey(addItem.Key);
                    if (product != null && !product.ProductVariants.Any()) variants.Add(product.AsMasterVariantDisplay());
                }
            }

            foreach (var v in variants.ToArray()) itemCache.AddItem(v);

            itemCache.Save();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Removes an item from the customer item cache.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ItemCacheLineItemDisplay}"/>.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage RemoveItemCacheItem(ItemCacheInstruction instruction)
        {
            var itemCache = GetCustomerItemCache(instruction);

            if (itemCache == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            itemCache.RemoveItem(instruction.EntityKey);

            itemCache.Save();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Moves an item from the customer basket to the wish list.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage MoveToWishlist(ItemCacheInstruction instruction)
        {
            var customer = GetCustomer(instruction);

            if (customer == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var basket = customer.Basket();

            basket.MoveItemToWishList(instruction.EntityKey);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Move an item from the customer wish list to the basket.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage MoveToBasket(ItemCacheInstruction instruction)
        {
            var customer = GetCustomer(instruction);

            if (customer == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var wishlist = customer.WishList();

            wishlist.MoveItemToBasket(instruction.EntityKey);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Updates a line item quantity.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage UpdateLineItemQuantity(ItemCacheInstruction instruction)
        {
            var itemCache = GetCustomerItemCache(instruction);

            if (itemCache == null) return Request.CreateResponse(HttpStatusCode.NotFound);
            

            var item = itemCache.Items.FirstOrDefault(x => x.Key == instruction.EntityKey);

            if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            itemCache.Items.First(x => x.Key == instruction.EntityKey).Quantity = instruction.Quantity;

            itemCache.Save();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Creates a customer invoice from the customer cart, default addresses and selected ship method.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public InvoiceDisplay CreateCheckoutInvoice(CreateCustomerInvoice model)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                // Get the customer
                var customer = _customerService.GetByKey(model.CustomerKey);
                if (customer == null) throw new NullReferenceException("Customer not found");

                // In this implementation, we require the customer addresses are populated which both a default billing and shipping address
                if (!customer.Addresses.Any()) throw new NullReferenceException("Customer does not have any saved addresses");

                var billing = customer.DefaultCustomerAddress(AddressType.Billing);
                var shipping = customer.DefaultCustomerAddress(AddressType.Shipping);

                if (billing == null) throw new NullReferenceException("Customer does not have a default billing address");
                if (shipping == null) throw new NullReferenceException("Customer does not have a default shipping address");

                // The customer basket must have items to purchase
                var basket = customer.Basket();
                if (basket.IsEmpty) throw new InvalidOperationException("Cannot complete back office checkout for customer without items in their basket.");

                // Use the basket checkout manager for facilate the checkout
                var checkoutManager = basket.GetCheckoutManager();
                checkoutManager.Customer.SaveBillToAddress(
                    billing.AsAddress(string.Format("{0} {1}", customer.FirstName, customer.LastName)));

                checkoutManager.Customer.SaveShipToAddress(
                    shipping.AsAddress(string.Format("{0} {1}", customer.FirstName, customer.LastName)));

                // quote the shipment and then save it to the checkout manager
                var shipment = basket.PackageBasket(checkoutManager.Customer.GetShipToAddress()).FirstOrDefault();
                var quote = shipment.ShipmentRateQuoteByShipMethod(model.ShipMethodKey, false);

                checkoutManager.Shipping.SaveShipmentRateQuote(quote);

                // We are not going to pay through the back office checkout, rather redirect to the sales overview page 
                // and pay from there since the payments can be implemented there
                var invoice = checkoutManager.Payment.PrepareInvoice();

                MerchelloContext.Services.InvoiceService.Save(invoice);

                customer.Basket().Empty();

                return invoice.ToInvoiceDisplay();
            }
            catch (Exception ex)
            {
                MultiLogHelper.Error<BackOfficeCheckoutApiController>("Failed to create customer invoice", ex);
                throw;
            }
        }

        #endregion

        /// <summary>
        /// The get customer item cache.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerItemCacheBase"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference if the customer is not found
        /// </exception>
        private CustomerItemCacheBase GetCustomerItemCache(ItemCacheInstructionBase instruction)
        {
            var customer = GetCustomer(instruction);

            if (customer == null)
            {
                var notFound = new NullReferenceException("Customer was not found");
                MultiLogHelper.Error<BackOfficeCheckoutApiController>("Could not add item to customer ItemCache", notFound);
                return null;
            }

            return instruction.ItemCacheType == ItemCacheType.Wishlist
                       ? (CustomerItemCacheBase)customer.WishList()
                       : (CustomerItemCacheBase)customer.Basket();
        }

        /// <summary>
        /// Gets the customer.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomer"/>.
        /// </returns>
        private ICustomer GetCustomer(ItemCacheInstructionBase instruction)
        {
            return _customerService.GetByKey(instruction.CustomerKey);
        }
    }
}