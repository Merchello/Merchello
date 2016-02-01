namespace Merchello.Web.Workflow
{
    using System;

    using Core;
    using Core.Cache;
    using Core.Models;
    using Core.Models.TypeFields;

    using Merchello.Web.Workflow.CustomerItemCache;

    using Umbraco.Core;

    /// <summary>
    /// Represents a shopping Basket or Cart
    /// </summary>
    public class Basket : CustomerItemCacheBase, IBasket
    {      
        /// <summary>
        /// Initializes a new instance of the <see cref="Basket"/> class.
        /// </summary>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        internal Basket(IItemCache itemCache, ICustomerBase customer)
            : base(itemCache, customer)
        {
            Mandate.ParameterCondition(itemCache.ItemCacheType == ItemCacheType.Basket, "itemCache");
        }

        /// <summary>
        /// Gets the sum of all basket item "amount" (price)
        /// </summary>
        public decimal TotalBasketPrice
        {
            get
            {
                return TotalItemCachePrice;
            }
        }

        /// <summary>
        /// Static method to instantiate a customer's basket
        /// </summary>
        /// <param name="customer">The customer associated with the basket</param>
        /// <returns>The customer's <see cref="IBasket"/></returns>
        public static IBasket GetBasket(ICustomerBase customer)
        {
            return GetBasket(MerchelloContext.Current, customer);
        }

        /// <summary>
        /// Refreshes the runtime cache
        /// </summary>
        /// <param name="merchelloContext">The merchello context</param>
        /// <param name="basket">The <see cref="IBasket"/></param>
        public static void Refresh(IMerchelloContext merchelloContext, IBasket basket)
        {
            var cacheKey = MakeCacheKey(basket.Customer);
            merchelloContext.Cache.RuntimeCache.ClearCacheItem(cacheKey);

            var customerItemCache = merchelloContext.Services.ItemCacheService.GetItemCacheWithKey(basket.Customer, ItemCacheType.Basket);
            basket = new Basket(customerItemCache, basket.Customer);
            merchelloContext.Cache.RuntimeCache.GetCacheItem(cacheKey, () => basket);
        }
  

        /// <summary>
        /// Empties the basket
        /// </summary>
        public override void Empty()
        {
            Empty(MerchelloContext.Current, this);
        }       

        /// <summary>
        /// Refreshes cache with database values
        /// </summary>
        public override void Refresh()
        {
           Refresh(MerchelloContext.Current, this);
        }

        /// <summary>
        /// Saves the basket
        /// </summary>
        public override void Save()
        {
            Save(MerchelloContext.Current, this);
        }

        /// <summary>
        /// Instantiates a basket
        /// </summary>
        /// <param name="merchelloContext">The merchello context</param>
        /// <param name="customer">The customer associated with the basket</param>
        /// <returns>The <see cref="IBasket"/></returns>
        internal static IBasket GetBasket(IMerchelloContext merchelloContext, ICustomerBase customer)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(customer, "customer");

            var cacheKey = MakeCacheKey(customer);

            var basket = (IBasket)merchelloContext.Cache.RuntimeCache.GetCacheItem(cacheKey);

            if (basket != null) return basket;

            var customerItemCache = merchelloContext.Services.ItemCacheService.GetItemCacheWithKey(customer, ItemCacheType.Basket);

            basket = new Basket(customerItemCache, customer);

            if (basket.Validate())
            {
                merchelloContext.Cache.RuntimeCache.GetCacheItem(cacheKey, () => basket);
            }

            return basket;
        }

        /// <summary>
        /// Instantiates the basket
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        /// <returns>
        /// The <see cref="IBasket"/>.
        /// </returns>
        internal static IBasket GetBasket(ICustomerBase customer, IItemCache itemCache)
        {
            return new Basket(itemCache, customer);
        }

        /// <summary>
        /// Empties the basket.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="basket">
        /// The basket.
        /// </param>
        internal static void Empty(IMerchelloContext merchelloContext, IBasket basket)
        {
            basket.Items.Clear();
            Save(merchelloContext, basket);
        }

        /// <summary>
        /// Saves the basket.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="basket">
        /// The basket.
        /// </param>
        internal static void Save(IMerchelloContext merchelloContext, IBasket basket)
        {
            // Update the basket item cache version so that it can be validated in the checkout
            ((Basket)basket).VersionKey = Guid.NewGuid();

            merchelloContext.Services.ItemCacheService.Save(((Basket)basket).ItemCache);

            Refresh(merchelloContext, basket);
        }
     
        /// <summary>
        /// Generates a unique cache key for runtime caching of the <see cref="Basket"/>
        /// </summary>
        /// <param name="customer">The <see cref="ICustomerBase"/></param>        
        /// <returns>The cache key for the customer's basket</returns>
        private static string MakeCacheKey(ICustomerBase customer)
        {
            // the version key here is not important since there can only ever be one basket
            return CacheKeys.ItemCacheCacheKey(customer.Key, EnumTypeFieldConverter.ItemItemCache.Basket.TypeKey, Guid.Empty);
        }
    }
}