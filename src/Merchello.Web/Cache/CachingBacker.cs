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
    }
}