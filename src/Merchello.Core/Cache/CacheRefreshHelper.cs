using System;
using Merchello.Core.Models;
using Umbraco.Core.Cache;

namespace Merchello.Core.Cache
{
    /// <summary>
    /// This class has been added to assist in correcting caching problems 
    /// </summary>
    internal class CacheRefreshHelper
    {
        public static void ClearProductFromCache(IRuntimeCacheProvider cache, Guid key)
        {
            cache.ClearCacheItem(CacheKeys.GetEntityCacheKey<IProduct>(key));
        }
    }
}