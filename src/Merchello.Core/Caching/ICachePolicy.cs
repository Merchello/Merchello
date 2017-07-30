namespace Merchello.Core.Caching
{
    using System.Collections.Generic;

    internal interface ICachePolicy<TItem>
    {
        void AddToCache(string cacheKey, TItem item);

        TItem GetFromCache(string cacheKey, IDictionary<string, object> options);
    }
}