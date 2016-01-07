namespace Merchello.Core.Checkout
{
    using System;

    using Merchello.Core.Gateways;
    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Sales;
    using Merchello.Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Events;

    /// <summary>
    /// Merchello's default checkout context.
    /// </summary>
    public class CheckoutContext : ICheckoutContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutContext"/> class.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        public CheckoutContext(ICustomerBase customer, IItemCache itemCache)
            : this(customer, itemCache, MerchelloContext.Current)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutContext"/> class.
        /// </summary>
        /// <param name="customer">
        /// The <see cref="ICustomerBase"/> associated with this checkout.
        /// </param>
        /// <param name="itemCache">
        /// The temporary <see cref="IItemCache"/> of the basket <see cref="IItemCache"/> to be used in the
        /// checkout process.
        /// </param>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        public CheckoutContext(ICustomerBase customer, IItemCache itemCache, IMerchelloContext merchelloContext)
        {
            Mandate.ParameterNotNull(customer, "customer");
            Mandate.ParameterNotNull(itemCache, "itemCache");
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");

            this.Services = merchelloContext.Services;
            this.Gateways = merchelloContext.Gateways;
            this.ItemCache = itemCache;
            this.Customer = customer;
            this.Cache = merchelloContext.Cache.RuntimeCache;
        }

        /// <summary>
        /// An event that fires when the context needs to be reset.
        /// </summary>
        public event TypedEventHandler<CheckoutContext, CheckoutContextEventArgs> Reset; 

        /// <summary>
        /// Gets the <see cref="IServiceContext"/>.
        /// </summary>
        public IServiceContext Services { get; private set; }

        /// <summary>
        /// Gets the <see cref="IGatewayContext"/>.
        /// </summary>
        public IGatewayContext Gateways { get; private set; }

        /// <summary>
        /// Gets the <see cref="IItemCache"/>.
        /// </summary>
        public IItemCache ItemCache { get; private set; }

        /// <summary>
        /// Gets the <see cref="ICustomerBase"/>.
        /// </summary>
        public ICustomerBase Customer { get; private set; }

        /// <summary>
        /// Gets the version key.
        /// </summary>
        /// <remarks>
        /// This is used for validation purposes to assert that the customer has not made changes to their basket/cart
        /// and thus require certain checkout process (such as shipping rates and taxation) do not need to be recalculated.
        /// </remarks>
        public virtual Guid VersionKey
        {
            get
            {
                return ItemCache.VersionKey;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to apply taxes to invoice.
        /// </summary>
        /// <remarks>
        /// Setting is only valid if store setting is set to apply taxes to the invoice and is NOT used
        /// when taxes are included in the product pricing.
        /// </remarks>
        public bool ApplyTaxesToInvoice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether raise customer events.
        /// </summary>
        /// <remarks>
        /// In some implementations, there may be quite a few saves to the customer record.  Use case for setting this to 
        /// false would be an API notification on a customer record change to prevent spamming of the notification.
        /// </remarks>
        public bool RaiseCustomerEvents { get; set; }

        /// <summary>
        /// Gets the <see cref="IRuntimeCacheProvider"/>.
        /// </summary>
        protected IRuntimeCacheProvider Cache { get; private set; }

        /// <summary>
        /// Gets the checkout <see cref="IItemCache"/> for the <see cref="ICustomerBase"/>
        /// </summary>
        /// <returns>
        /// The <see cref="IItemCache"/> associated with the customer checkout
        /// </returns>
        protected virtual IItemCache GetItemCache()
        {
            var cacheKey = MakeCacheKey();
            var itemCache = Cache.GetCacheItem(cacheKey) as IItemCache;
            if (itemCache != null) return itemCache;

            itemCache = Services.ItemCacheService.GetItemCacheWithKey(Customer, ItemCacheType.Checkout, VersionKey);

            // this is probably an invalid version of the checkout
            if (!itemCache.VersionKey.Equals(VersionKey))
            {
                var oldCacheKey = MakeCacheKey();
                Cache.ClearCacheItem(oldCacheKey);
                Reset.RaiseEvent(new CheckoutContextEventArgs(Customer, itemCache), this);

                // delete the old version
                Services.ItemCacheService.Delete(itemCache);
                return GetItemCache();
            }

            Cache.InsertCacheItem(cacheKey, () => itemCache);

            return itemCache;
        }

        /// <summary>
        /// Generates a unique cache key for runtime caching of the <see cref="SalePreparationBase"/>
        /// </summary>
        /// <returns>
        /// The a string used as a runtime cache key.
        /// </returns>
        /// <remarks>
        /// 
        /// CacheKey is assumed to be unique per customer and globally for CheckoutBase.  Therefore this will NOT be unique if 
        /// to different checkouts are happening for the same customer at the same time - we consider that an extreme edge case.
        /// 
        /// </remarks>
        private string MakeCacheKey()
        {
            var itemCacheTfKey = EnumTypeFieldConverter.ItemItemCache.Checkout.TypeKey;
            return Core.Cache.CacheKeys.ItemCacheCacheKey(Customer.Key, itemCacheTfKey, VersionKey);
        }
    }
}