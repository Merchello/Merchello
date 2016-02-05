namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Merchello.Core;
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
        public HttpResponseMessage AddItemCacheItem(ItemCacheInstruction instruction)
        {
            var itemCache = GetCustomerItemCache(instruction);

            if (itemCache == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var variant = _merchello.Query.Product.GetProductVariantByKey(instruction.EntityKey);

            if (variant == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            itemCache.AddItem(variant);

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
        private CustomerItemCacheBase GetCustomerItemCache(ItemCacheInstruction instruction)
        {
            var customer = GetCustomer(instruction);

            if (customer == null)
            {
                var notFound = new NullReferenceException("Customer was not found");
                LogHelper.Error<BackOfficeCheckoutApiController>("Could not add item to customer ItemCache", notFound);
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
        private ICustomer GetCustomer(ItemCacheInstruction instruction)
        {
            return _customerService.GetByKey(instruction.CustomerKey);
        }
    }
}