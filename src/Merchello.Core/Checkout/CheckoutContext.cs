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
            : this(customer, itemCache, new CheckoutContextSettings())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutContext"/> class.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public CheckoutContext(ICustomerBase customer, IItemCache itemCache, ICheckoutContextSettings settings)
            : this(customer, itemCache, Core.MerchelloContext.Current, settings)
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
        /// <param name="settings">
        /// The version change settings.
        /// </param>
        public CheckoutContext(ICustomerBase customer, IItemCache itemCache, IMerchelloContext merchelloContext, ICheckoutContextSettings settings)
        {
            Mandate.ParameterNotNull(customer, "customer");
            Mandate.ParameterNotNull(itemCache, "itemCache");
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(settings, "settings");

            this.MerchelloContext = merchelloContext;
            this.ItemCache = itemCache;
            this.Customer = customer;
            this.Cache = merchelloContext.Cache.RuntimeCache;
            this.ApplyTaxesToInvoice = true;
            this.Settings = settings;
            this.RaiseCustomerEvents = false;
        }

        /// <summary>
        /// Gets the merchello context.
        /// </summary>
        public IMerchelloContext MerchelloContext { get; private set; }

        /// <summary>
        /// Gets the <see cref="IServiceContext"/>.
        /// </summary>
        public IServiceContext Services
        {
            get
            {
                return MerchelloContext.Services;
            }
        }

        /// <summary>
        /// Gets the <see cref="IGatewayContext"/>.
        /// </summary>
        public IGatewayContext Gateways
        {
            get
            {
                return MerchelloContext.Gateways;
            }
        }

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
        /// Gets a value indicating whether is new version.
        /// </summary>
        public bool IsNewVersion { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to apply taxes to invoice.
        /// </summary>
        /// <remarks>
        /// Setting is only valid if store setting is set to apply taxes to the invoice and is NOT used
        /// when taxes are included in the product pricing.
        /// </remarks>
        public bool ApplyTaxesToInvoice { get; set; }

        /// <summary>
        /// Gets or sets a prefix to be prepended to an invoice number.
        /// </summary>
        public string InvoiceNumberPrefix { get; set; }

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
        public IRuntimeCacheProvider Cache { get; private set; }

        /// <summary>
        /// Gets the version change settings.
        /// </summary>
        public ICheckoutContextSettings Settings { get; private set; }

        /// <summary>
        /// Gets the <see cref="ICheckoutContext"/> for the customer.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="versionKey">
        /// The version key.
        /// </param>
        /// <returns>
        /// The <see cref="ICheckoutContext"/>.
        /// </returns>
        public static ICheckoutContext CreateCheckoutContext(ICustomerBase customer, Guid versionKey)
        {
            return CreateCheckoutContext(customer, versionKey, new CheckoutContextSettings());
        }

        /// <summary>
        /// Gets the <see cref="ICheckoutContext"/> for the customer.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="versionKey">
        /// The version key.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="ICheckoutContext"/>.
        /// </returns>
        public static ICheckoutContext CreateCheckoutContext(ICustomerBase customer, Guid versionKey, ICheckoutContextSettings settings)
        {
            return CreateCheckoutContext(Core.MerchelloContext.Current, customer, versionKey, settings);
        }

        /// <summary>
        /// Gets the <see cref="ICheckoutContext"/> for the customer
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello Context.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="versionKey">
        /// The version Key.
        /// </param>
        /// <param name="settings">
        /// The checkout context version change settings.
        /// </param>
        /// <returns>
        /// The <see cref="ICheckoutContext"/> associated with the customer checkout
        /// </returns>
        public static ICheckoutContext CreateCheckoutContext(IMerchelloContext merchelloContext, ICustomerBase customer, Guid versionKey, ICheckoutContextSettings settings)
        {
            var cache = merchelloContext.Cache.RuntimeCache;
            var cacheKey = MakeCacheKey(customer, versionKey);
            var itemCache = cache.GetCacheItem(cacheKey) as IItemCache;
            if (itemCache != null) return new CheckoutContext(customer, itemCache, merchelloContext, settings);

            itemCache = merchelloContext.Services.ItemCacheService.GetItemCacheWithKey(customer, ItemCacheType.Checkout, versionKey);

            // this is probably an invalid version of the checkout
            if (!itemCache.VersionKey.Equals(versionKey))
            {
                var oldCacheKey = MakeCacheKey(customer, versionKey);
                cache.ClearCacheItem(oldCacheKey);

                // delete the old version
                merchelloContext.Services.ItemCacheService.Delete(itemCache);
                return CreateCheckoutContext(merchelloContext, customer, versionKey, settings);
            }

            cache.InsertCacheItem(cacheKey, () => itemCache);

            return new CheckoutContext(customer, itemCache, merchelloContext, settings) { IsNewVersion = true };
        }


        /// <summary>
        /// Generates a unique cache key for runtime caching of the <see cref="IItemCache"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="versionKey">
        /// The version Key.
        /// </param>
        /// <returns>
        /// The a string used as a runtime cache key.
        /// </returns>
        /// <remarks>
        /// 
        /// CacheKey is assumed to be unique per customer and globally for CheckoutBase.  Therefore this will NOT be unique if 
        /// to different checkouts are happening for the same customer at the same time - we consider that an extreme edge case.
        /// 
        /// </remarks>
        private static string MakeCacheKey(ICustomerBase customer, Guid versionKey)
        {
            var itemCacheTfKey = EnumTypeFieldConverter.ItemItemCache.Checkout.TypeKey;
            return Core.Cache.CacheKeys.ItemCacheCacheKey(customer.Key, itemCacheTfKey, versionKey);
        }
    }
}