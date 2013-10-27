using System;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Web.Models;

namespace Merchello.Web.Cache
{
    internal static class CachingBacker
    {
        /// <summary>
        /// Returns a cache key intended for runtime caching of a <see cref="ICustomerBase"/>
        /// </summary>
        /// <param name="customerKey"></param>
        /// <returns></returns>
        public static string CostumerCacheKey(Guid customerKey)
        {
            return string.Format("merchello.consumer.{0}", customerKey);   
        }

        /// <summary>
        /// Returns a cache key intend for runtime caching of a <see cref="IBasket"/>
        /// </summary>
        /// <param name="customerKey"></param>
        /// <param name="itemCacheTfKey">The type field key for the cache</param>
        /// <returns></returns>
        public static string CustomerBasketCacheKey(Guid customerKey, Guid itemCacheTfKey)
        {
            return string.Format("merchello.itemcache.{0}.{1}", itemCacheTfKey, customerKey);
        }
    }
}