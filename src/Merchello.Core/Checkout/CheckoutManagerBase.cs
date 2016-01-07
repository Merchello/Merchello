namespace Merchello.Core.Checkout
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Sales;

    using Umbraco.Core;

    /// <summary>
    /// The checkout manager base.
    /// </summary>
    public abstract class CheckoutManagerBase : CheckoutContextManagerBase, ICheckoutManagerBase
    {
        /// <summary>
        /// The merchello context.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutManagerBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="checkoutContext">
        /// The checkout Context.
        /// </param>
        protected CheckoutManagerBase(IMerchelloContext merchelloContext, ICheckoutContext checkoutContext)
            : base(checkoutContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            
            _merchelloContext = merchelloContext;
        }

        public ICheckoutCustomerManager Customer { get; }

        public ICheckoutOfferManager Offer { get; }

        public ICheckoutShippingManager Shipping { get; }

        public ICheckoutPaymentManager Payment { get; }

        public ICheckoutCompletionManager Completion { get; }

        public ICheckoutNotificationManager Notification { get; }


        /// <summary>
        /// Gets the <see cref="IItemCache"/>
        /// </summary>
        public IItemCache ItemCache
        {
            get { return Context.ItemCache; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether raise customer events.
        /// </summary>
        public bool RaiseCustomerEvents { get; set; }


        /// <summary>
        /// Gets or sets a prefix to be prepended to an invoice number.
        /// </summary>
        public string InvoiceNumberPrefix { get; set; }

        /// <summary>
        /// Purges sales manager information
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Gets a clone of the ItemCache
        /// </summary>
        /// <returns>
        /// The <see cref="IItemCache"/>.
        /// </returns>
        internal IItemCache CloneItemCache()
        {
            // The ItemCache needs to be cloned as line items may be altered while applying constraints
            return this.CloneItemCache(ItemCache);
        }

        /// <summary>
        /// Clones a <see cref="ILineItemContainer"/> as <see cref="IItemCache"/>
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="IItemCache"/>.
        /// </returns>
        internal IItemCache CloneItemCache(ILineItemContainer container)
        {
            var clone = new ItemCache(Guid.NewGuid(), ItemCacheType.Backoffice);
            foreach (var item in container.Items)
            {
                clone.Items.Add(item.AsLineItemOf<ItemCacheLineItem>());
            }

            return clone;
        }

        /// <summary>
        /// Creates a new <see cref="ILineItemContainer"/> with filtered items.
        /// </summary>
        /// <param name="filteredItems">
        /// The line items.
        /// </param>
        /// <returns>
        /// The <see cref="ILineItemContainer"/>.
        /// </returns>
        internal ILineItemContainer CreateNewLineContainer(IEnumerable<ILineItem> filteredItems)
        {
            return LineItemExtensions.CreateNewItemCacheLineItemContainer(filteredItems);
        }

        /// <summary>
        /// Gets the checkout <see cref="IItemCache"/> for the <see cref="ICustomerBase"/>
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>
        /// </param>
        /// <param name="customer">
        /// The customer associated with the checkout
        /// </param>
        /// <param name="versionKey">
        /// The version key for this <see cref="SalePreparationBase"/>
        /// </param>
        /// <returns>
        /// The <see cref="IItemCache"/> associated with the customer checkout
        /// </returns>
        protected static IItemCache GetItemCache(IMerchelloContext merchelloContext, ICustomerBase customer, Guid versionKey)
        {
            var runtimeCache = merchelloContext.Cache.RuntimeCache;

            var cacheKey = MakeCacheKey(customer, versionKey);
            var itemCache = runtimeCache.GetCacheItem(cacheKey) as IItemCache;
            if (itemCache != null) return itemCache;

            itemCache = merchelloContext.Services.ItemCacheService.GetItemCacheWithKey(customer, ItemCacheType.Checkout, versionKey);

            // this is probably an invalid version of the checkout
            if (!itemCache.VersionKey.Equals(versionKey))
            {
                var oldCacheKey = MakeCacheKey(customer, itemCache.VersionKey);
                runtimeCache.ClearCacheItem(oldCacheKey);
                
                // TODO RSS figure out how to get this back in there
                // Reset(merchelloContext, customer);

                // delete the old version
                merchelloContext.Services.ItemCacheService.Delete(itemCache);
                return GetItemCache(merchelloContext, customer, versionKey);
            }

            runtimeCache.InsertCacheItem(cacheKey, () => itemCache);

            return itemCache;
        }

        /// <summary>
        /// Generates a unique cache key for runtime caching of the <see cref="SalePreparationBase"/>
        /// </summary>
        /// <param name="customer">The <see cref="ICustomerBase"/> for which to generate the cache key</param>
        /// <param name="versionKey">The version key</param>
        /// <returns>The unique CacheKey string</returns>
        /// <remarks>
        /// 
        /// CacheKey is assumed to be unique per customer and globally for CheckoutBase.  Therefore this will NOT be unique if 
        /// to different checkouts are happening for the same customer at the same time - we consider that an extreme edge case.
        /// 
        /// </remarks>
        private static string MakeCacheKey(ICustomerBase customer, Guid versionKey)
        {
            var itemCacheTfKey = EnumTypeFieldConverter.ItemItemCache.Checkout.TypeKey;
            return Cache.CacheKeys.ItemCacheCacheKey(customer.Key, itemCacheTfKey, versionKey);
        }

        /// <summary>
        /// Class that gets serialized to customer's ExtendedDataCollection to save offer code queue data.
        /// </summary>
        private struct OfferCodeTempData
        {
            /// <summary>
            /// Gets or sets the version key to validate offer codes are validate with this preparation
            /// </summary>
            public Guid VersionKey { get; set; }

            /// <summary>
            /// Gets or sets the offer codes.
            /// </summary>
            public IEnumerable<string> OfferCodes { get; set; }
        }
    }
}