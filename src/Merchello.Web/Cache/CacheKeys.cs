using System;
using Merchello.Core.Models;
using Merchello.Web.Models;
using Merchello.Web.Shipping;
using Merchello.Web.Shipping.Gateway.FlatRate;

namespace Merchello.Web.Cache
{
    internal static class CacheKeys
    {
        /// <summary>
        /// Returns a cache key intended for runtime caching of a <see cref="ICustomerBase"/>
        /// </summary>
        /// <param name="entityKey"></param>
        /// <returns></returns>
        public static string CostumerCacheKey(Guid entityKey)
        {
            return string.Format("merchello.customer.{0}", entityKey);   
        }

        /// <summary>
        /// Returns a cache key intend for runtime caching of a <see cref="IBasket"/>
        /// </summary>
        /// <param name="entityKey"></param>
        /// <param name="itemCacheTfKey">The type field key for the cache</param>
        /// <returns></returns>
        public static string CustomerBasketCacheKey(Guid entityKey, Guid itemCacheTfKey)
        {
            return string.Format("merchello.itemcache.{0}.{1}", itemCacheTfKey, entityKey);
        }

        /// <summary>
        /// Returns a cache key intended for runtime caching of a <see cref="IShipRateTable"/>
        /// </summary>
        /// <param name="shipMethodKey">The unique key (Guid) of the <see cref="IShipMethod"/> associated with the rate table</param>
        /// <returns></returns>
        public static string ShipRateTableCacheKey(Guid shipMethodKey)
        {
            return string.Format("merchello.shipmethod.shipratetable.{0}", shipMethodKey.ToString());
        }
    }
}