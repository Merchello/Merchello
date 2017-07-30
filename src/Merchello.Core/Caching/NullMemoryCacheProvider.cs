namespace Merchello.Core.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Merchello.Core.Configuration;

    public class NullMemoryCacheProvider : IMemoryCacheProvider
    {
        public NullMemoryCacheProvider(IMemoryCacheSettings settings)
        {
            this.Settings = settings;
        }

        public IMemoryCacheSettings Settings { get; }

        public void ClearAll()
        {
        }

        public void ClearItem(string key)
        {
        }

        [CanBeNull]
        public object GetItem(string key)
        {
            return null;
        }

        public object GetItem(string key, Func<object> getItem)
        {
            return getItem.Invoke();
        }

        public IEnumerable<object> GetItems(params string[] cacheKeys)
        {
            return Enumerable.Empty<object>();
        }

        public object GetItem(string cacheKey, Func<object> getItem, TimeSpan? timeout, bool isSliding = false)
        {
            return getItem.Invoke();
        }
    }
}