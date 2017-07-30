namespace Merchello.Core.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using JetBrains.Annotations;

    using Merchello.Core.Configuration;
    using Merchello.Core.Threading;

    using Microsoft.Extensions.Caching.Memory;

    internal class MemoryCacheProvider : IMemoryCacheProvider
    {
        private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private IMemoryCache memCache = Create();

        public MemoryCacheProvider(IMemoryCacheSettings settings)
        {
            this.Settings = settings;
            this.InstanceKey = Guid.NewGuid();
        }

        public IMemoryCacheSettings Settings { get; }

        /// Used in tests
        internal Guid InstanceKey { get; private set; }

        public void ClearAll()
        {
            using (new SlimWriterLock(this.locker))
            {
                this.memCache.Dispose();
                this.memCache = Create();
                this.InstanceKey = Guid.NewGuid();
            }
        }

        public void ClearItem(string key)
        {
            using (new SlimWriterLock(this.locker))
            {
                if (this.memCache.Get(key) == null)
                {
                    return;
                }
                this.memCache.Remove(key);
            }
        }

        [CanBeNull]
        public object GetItem(string key)
        {
            object result;
            bool success;
            using (new SlimWriterLock(this.locker))
            {
                success = this.memCache.TryGetValue(key, out result);
            }

            return success ? result : null;
        }

        public object GetItem(string key, Func<object> getItem)
        {
            return this.GetItem(key, getItem, TimeSpan.FromMinutes(5), false);
        }

        public IEnumerable<object> GetItems(params string[] cacheKeys)
        {
            if (!cacheKeys.Any())
            {
                return Enumerable.Empty<object>();
            }

            return cacheKeys.Select(this.GetItem).Where(x => x != null);
        }

        public object GetItem(string key, Func<object> getItem, TimeSpan? timeout, bool isSliding = false)
        {
            if (!this.memCache.TryGetValue(key, out object value))
            {
                value = getItem.Invoke();
                if (value != null)
                {
                    var options = new MemoryCacheEntryOptions();
                    if (timeout.HasValue)
                    {
                        if (isSliding)
                        {
                            options.SetSlidingExpiration(timeout.Value);
                        }
                        else
                        {
                            options.AbsoluteExpiration = DateTime.Now + timeout.Value;
                        }
                    }

                    this.memCache.Set(key, value, options);
                }
                else
                {
                    throw new NullReferenceException("Attempt to cache a null object.");
                }
            }

            return value;
        }

        private static IMemoryCache Create()
        {
            return new MemoryCache(new MemoryCacheOptions { ExpirationScanFrequency = TimeSpan.FromMinutes(5) });
        }
    }
}