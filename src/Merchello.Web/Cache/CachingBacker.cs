using System;
using Merchello.Core.Models;
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
        public static string ConsumerCacheKey(Guid customerKey)
        {
            return string.Format("merchello.consumer.{0}", customerKey);   
        }

        /// <summary>
        /// Returns a cache key intend for runtime caching of a <see cref="IBasket"/>
        /// </summary>
        /// <param name="customerKey"></param>
        /// <returns></returns>
        public static string BasketCacheKey(Guid customerKey)
        {
            return string.Format("merchello.basket.{0}", customerKey);
        }
    }
}