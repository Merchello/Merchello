namespace Merchello.Core.Cache
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a cache provider that statically caches item in a concurrent dictionary.
    /// </summary>
    /// UMBRACO_SRC Direct port of Umbraco internal interface to get rid of hard dependency
    public class StaticCacheProvider : ICacheProvider
    {
        internal readonly ConcurrentDictionary<string, object> StaticCache = new ConcurrentDictionary<string, object>();

        /// <inheritdoc/>
        public virtual void ClearAllCache()
        {
            this.StaticCache.Clear();
        }

        /// <inheritdoc/>
        public virtual void ClearCacheItem(string key)
        {
            object val;
            this.StaticCache.TryRemove(key, out val);
        }

        /// <inheritdoc/>
        public virtual void ClearCacheObjectTypes(string typeName)
        {
            this.StaticCache.RemoveAll(kvp => kvp.Value != null && kvp.Value.GetType().ToString().InvariantEquals(typeName));
        }

        /// <inheritdoc/>
        public virtual void ClearCacheObjectTypes<T>()
        {
            var typeOfT = typeof(T);
            this.StaticCache.RemoveAll(kvp => kvp.Value != null && kvp.Value.GetType() == typeOfT);
        }

        /// <inheritdoc/>
        public virtual void ClearCacheObjectTypes<T>(Func<string, T, bool> predicate)
        {
            var typeOfT = typeof(T);
            this.StaticCache.RemoveAll(kvp => kvp.Value != null && kvp.Value.GetType() == typeOfT && predicate(kvp.Key, (T)kvp.Value));
        }

        /// <inheritdoc/>
        public virtual void ClearCacheByKeySearch(string keyStartsWith)
        {
            this.StaticCache.RemoveAll(kvp => kvp.Key.InvariantStartsWith(keyStartsWith));
        }

        /// <inheritdoc/>
        public virtual void ClearCacheByKeyExpression(string regexString)
        {
            this.StaticCache.RemoveAll(kvp => Regex.IsMatch(kvp.Key, regexString)); 
        }

        /// <inheritdoc/>
        public virtual IEnumerable<object> GetCacheItemsByKeySearch(string keyStartsWith)
        {
            return (from KeyValuePair<string, object> c in this.StaticCache
                    where c.Key.InvariantStartsWith(keyStartsWith)
                    select c.Value).ToList();
        }

        /// <inheritdoc/>
        public IEnumerable<object> GetCacheItemsByKeyExpression(string regexString)
        {
            return (from KeyValuePair<string, object> c in this.StaticCache
                    where Regex.IsMatch(c.Key, regexString) 
                    select c.Value).ToList();
        }

        /// <inheritdoc/>
        public virtual object GetCacheItem(string cacheKey)
        {
            var result = this.StaticCache[cacheKey];
            return result;
        }

        /// <inheritdoc/>
        public virtual object GetCacheItem(string cacheKey, Func<object> getCacheItem)
        {
            return this.StaticCache.GetOrAdd(cacheKey, key => getCacheItem());
        }        
    }
}