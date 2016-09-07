namespace Merchello.Core.Acquired.Cache.Providers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using Merchello.Core.Acquired.Threading;

    /// <summary>
    /// A cache provider that caches items in the HttpContext.Items
    /// </summary>
    /// UMBRACO_SRC Direct port of Umbraco internal interface to get rid of hard dependency
    internal class HttpRequestCacheProvider : DictionaryCacheProviderBase
    {

        /// <summary>
        /// The context provider.
        /// </summary>
        /// <remarks>
        /// <para>The idea is that there is only one, application-wide HttpRequestCacheProvider instance,
        /// that is initialized with a method that returns the "current" context.
        /// </para>
        /// <para>
        ///  NOTE
        ///   but then it is initialized with () => new HttpContextWrapper(HttpContent.Current)
        ///   which is highly inefficient because it creates a new wrapper each time we refer to _context()
        ///   so replace it with _context1 and _context2 below + a way to get context.Items.
        /// private readonly <see cref="Func{HttpContextBase}"/> _context;
        /// </para>
        /// <para> 
        /// NOTE
        ///   and then in almost 100% cases _context2 will be () => HttpContext.Current
        ///   so why not bring that logic in here and fallback on to HttpContext.Current when
        ///   _context1 is null?
        /// private readonly HttpContextBase _context1;
        /// private readonly <see cref="Func{HttpContext}"/> _context2;
        /// </para>
        /// </remarks>
        private readonly HttpContextBase _context;

        private IDictionary ContextItems
        {
            //get { return _context1 != null ? _context1.Items : _context2().Items; }
            get { return this._context != null ? this._context.Items : HttpContext.Current.Items; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestCacheProvider"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <remarks>
        /// for unit tests
        /// </remarks>
        public HttpRequestCacheProvider(HttpContextBase context)
        {
            this._context = context;
        }

        // main constructor
        // will use HttpContext.Current
        public HttpRequestCacheProvider(/*Func<HttpContext> context*/)
        {
            //_context2 = context;
        }

        protected override IEnumerable<DictionaryEntry> GetDictionaryEntries()
        {
            const string Prefix = CacheItemPrefix + "-";

            return this.ContextItems.Cast<DictionaryEntry>()
                .Where(x => x.Key is string && ((string)x.Key).StartsWith(Prefix));
        }

        protected override void RemoveEntry(string key)
        {
            this.ContextItems.Remove(key);
        }

        protected override object GetEntry(string key)
        {
            return this.ContextItems[key];
        }

        #region Lock

        protected override IDisposable ReadLock
        {
            // there's no difference between ReadLock and WriteLock here
            get { return this.WriteLock; }
        }

        protected override IDisposable WriteLock
        {
            // NOTE
            //   could think about just overriding base.Locker to return a different
            //   object but then we'd create a ReaderWriterLockSlim per request,
            //   which is less efficient than just using a basic monitor lock.

            get
            {
                return new MonitorLock(this.ContextItems.SyncRoot);
            }
        }

        #endregion

        #region Get

        public override object GetCacheItem(string cacheKey, Func<object> getCacheItem)
        {
            cacheKey = this.GetCacheKey(cacheKey);

            Lazy<object> result;

            using (this.WriteLock)
            {
                result = this.ContextItems[cacheKey] as Lazy<object>; // null if key not found

                // cannot create value within the lock, so if result.IsValueCreated is false, just
                // do nothing here - means that if creation throws, a race condition could cause
                // more than one thread to reach the return statement below and throw - accepted.

                if (result == null || GetSafeLazyValue(result, true) == null) // get non-created as NonCreatedValue & exceptions as null
                {
                    result = GetSafeLazy(getCacheItem);
                    this.ContextItems[cacheKey] = result;
                }
            }

            // using GetSafeLazy and GetSafeLazyValue ensures that we don't cache
            // exceptions (but try again and again) and silently eat them - however at
            // some point we have to report them - so need to re-throw here

            // this does not throw anymore
            //return result.Value;

            var value = result.Value; // will not throw (safe lazy)
            var eh = value as ExceptionHolder;
            if (eh != null) throw eh.Exception; // throw once!
            return value;
        }

        #endregion

        #region Insert
        #endregion

    }
}