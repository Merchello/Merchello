using System;
using Merchello.Core.Models;

namespace Merchello.Web.Cache
{
    internal static class CachingBacker
    {
        /// <summary>
        /// Returns a cache key intended for runtime caching of a <see cref="IConsumer"/>
        /// </summary>
        /// <param name="consumerKey"></param>
        /// <returns></returns>
        public static string ConsumerCacheKey(Guid consumerKey)
        {
            return string.Format("merchello.consumer.{0}", consumerKey);   
        }
    }
}