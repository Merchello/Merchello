namespace Merchello.Web.Workflow
{
    using System;
    using System.Linq;
    using Core;
    using Core.Cache;
    using Core.Models;
    using Core.Models.TypeFields;

    using Merchello.Web.Workflow.CustomerItemCache;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents a shopping wish list or Cart
    /// </summary>
    public class WishList : CustomerItemCacheBase, IWishList
    { 
       
        /// <summary>
        /// Initializes a new instance of the <see cref="WishList"/> class.
        /// </summary>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        internal WishList(IItemCache itemCache, ICustomerBase customer)
            : base(itemCache, customer)
        {
            Ensure.ParameterCondition(itemCache.ItemCacheType == ItemCacheType.Wishlist, "itemCache");
        }
       
        /// <summary>
        /// Gets the sum of all wish list item "amount" (price)
        /// </summary>
        public decimal TotalWishListPrice
        {
            get { return Items.Sum(x => (x.Quantity * x.Price)); }
        }


        /// <summary>
        /// Static method to instantiate a customer's basket
        /// </summary>
        /// <param name="customer">The customer associated with the basket</param>
        /// <returns>The customer's <see cref="IBasket"/></returns>
        public static IWishList GetWishList(ICustomerBase customer)
        {
            return GetWishList(MerchelloContext.Current, customer);
        }

        /// <summary>
        /// Refreshes the runtime cache
        /// </summary>
        /// <param name="merchelloContext">The merchello context</param>
        /// <param name="wishlist">The <see cref="IWishList"/></param>
        public static void Refresh(IMerchelloContext merchelloContext, IWishList wishlist)
        {
            var cacheKey = MakeCacheKey(wishlist.Customer);
            merchelloContext.Cache.RuntimeCache.ClearCacheItem(cacheKey);

            var customerItemCache = merchelloContext.Services.ItemCacheService.GetItemCacheWithKey(wishlist.Customer, ItemCacheType.Wishlist);
            wishlist = new WishList(customerItemCache, wishlist.Customer);
            merchelloContext.Cache.RuntimeCache.GetCacheItem(cacheKey, () => wishlist, TimeSpan.FromHours(2));
        }

        /// <summary>
        /// Static method to instantiate a customer's wish list
        /// </summary>
        /// <param name="loginName">The customers login name associated with the wish list</param>
        /// <returns>The customer's <see cref="IWishList"/></returns>
        public static IWishList GetWishList(string loginName)
        {
            return GetWishList(MerchelloContext.Current, loginName);
        }   
        
        /// <summary>
        /// Static method to instantiate a customer's wish list
        /// </summary>
        /// <param name="customerKey">The customers login name associated with the wish list</param>
        /// <returns>The customer's <see cref="IWishList"/></returns>
        public static IWishList GetWishList(Guid customerKey)
        {
            return GetWishList(MerchelloContext.Current, customerKey);
        }        

        /// <summary>
        /// Empties the wish list
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
        /// Saves the wish list
        /// </summary>
        public override void Save()
        {
            Save(MerchelloContext.Current, this);
        }

        /// <summary>
        /// Instantiates a wish list
        /// </summary>
        /// <param name="merchelloContext">The merchello context</param>
        /// <param name="loginName">The customers login name associated with the wish list</param>
        /// <returns>The <see cref="IWishList"/></returns>
        internal static IWishList GetWishList(IMerchelloContext merchelloContext, string loginName)
        {
            Ensure.ParameterNotNull(merchelloContext, "merchelloContext");
            
            var customer = merchelloContext.Services.CustomerService.GetByLoginName(loginName);
            return customer == null ? null : GetWishList(merchelloContext, customer);
        }

        /// <summary>
        /// Instantiates a wish list
        /// </summary>
        /// <param name="merchelloContext">The merchello context</param>
        /// <param name="customerKey">The customers key associated with the wish list</param>
        /// <returns>The <see cref="IWishList"/></returns>
        internal static IWishList GetWishList(IMerchelloContext merchelloContext, Guid customerKey)
        {
            Ensure.ParameterNotNull(merchelloContext, "merchelloContext");

            var customer = merchelloContext.Services.CustomerService.GetByKey(customerKey);
            return customer == null ? null : GetWishList(merchelloContext, customer);
        }

        /// <summary>
        /// Instantiates a wish list
        /// </summary>
        /// <param name="merchelloContext">The merchello context</param>
        /// <param name="customer">The customer associated with the wish list</param>
        /// <returns>The <see cref="IWishList"/></returns>
        internal static IWishList GetWishList(IMerchelloContext merchelloContext, ICustomerBase customer)
        {
            Ensure.ParameterNotNull(merchelloContext, "merchelloContext");
            Ensure.ParameterNotNull(customer, "customer");

            var cacheKey = MakeCacheKey(customer);

            var wishlist = (IWishList)merchelloContext.Cache.RuntimeCache.GetCacheItem(cacheKey);

            if (wishlist != null && wishlist.Validate()) return wishlist;

            var customerItemCache = merchelloContext.Services.ItemCacheService.GetItemCacheWithKey(customer, ItemCacheType.Wishlist);

            wishlist = new WishList(customerItemCache, customer);

            if (wishlist.Validate())
            {
                merchelloContext.Cache.RuntimeCache.GetCacheItem(cacheKey, () => wishlist, TimeSpan.FromHours(2));    
            }
            
            return wishlist;
        }

        /// <summary>
        /// Gets the wish list.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        /// <returns>
        /// The <see cref="IWishList"/>.
        /// </returns>
        internal static IWishList GetWishList(ICustomerBase customer, IItemCache itemCache)
        {
            return new WishList(itemCache, customer);
        }

        /// <summary>
        /// Empties the wish list.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="wishlist">
        /// The wish list.
        /// </param>
        internal static void Empty(IMerchelloContext merchelloContext, IWishList wishlist)
        {
            wishlist.Items.Clear();
            Save(merchelloContext, wishlist);
        }

        /// <summary>
        /// Saves the wish list.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="wishlist">
        /// The wish list.
        /// </param>
        internal static void Save(IMerchelloContext merchelloContext, IWishList wishlist)
        {
            // Update the wishlist item cache version so that it can be validated in the checkout
            ((WishList)wishlist).VersionKey = Guid.NewGuid();

            merchelloContext.Services.ItemCacheService.Save(((WishList)wishlist).ItemCache);

            Refresh(merchelloContext, wishlist);
        }
     
        /// <summary>
        /// Generates a unique cache key for runtime caching of the <see cref="WishList"/>
        /// </summary>
        /// <param name="customer">The <see cref="ICustomerBase"/></param>        
        /// <returns>The cache key for the customer's wish list</returns>
        private static string MakeCacheKey(ICustomerBase customer)
        {
            // the version key here is not important since there can only ever be one wishlist
            return CacheKeys.ItemCacheCacheKey(customer.Key, EnumTypeFieldConverter.ItemItemCache.Wishlist.TypeKey, Guid.Empty);
        }
    }
}