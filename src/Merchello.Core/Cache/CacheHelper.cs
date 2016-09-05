namespace Merchello.Core.Cache
{
    using System;
    using System.Web;

    /// <summary>
    /// The cache manager.
    /// </summary>
    /// FYI - this needs to remain internal so we can rely on CMS caching providers
    /// TODO REFACTOR rename this class after port has been completed
    internal class CacheHelper : ICacheHelper
    {
        /// <summary>
        /// A <see cref="NullCacheProvider"/> for use with the request cache.
        /// </summary>
        private static readonly ICacheProvider NullRequestCache = new NullCacheProvider();

        /// <summary>
        /// A <see cref="NullCacheProvider"/> for use with the static cache.
        /// </summary>
        private static readonly ICacheProvider NullStaticCache = new NullCacheProvider();

        /// <summary>
        /// A <see cref="NullCacheProvider"/> for use with the run time cache.
        /// </summary>
        private static readonly IRuntimeCacheProvider NullRuntimeCache = new NullCacheProvider();


        /// <summary>
        /// Initializes a new instance of the <see cref="CacheHelper"/> class for use in the web. 
        /// </summary>
        public CacheHelper()
            : this(
                new HttpRuntimeCacheProvider(HttpRuntime.Cache),
                new StaticCacheProvider(),
                new HttpRequestCacheProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheHelper"/> class for use in the web
        /// </summary>
        /// <param name="cache">
        /// The <see cref="System.Web.Caching.Cache"/>
        /// </param>
        public CacheHelper(System.Web.Caching.Cache cache)
            : this(
                new HttpRuntimeCacheProvider(cache),
                new StaticCacheProvider(),
                new HttpRequestCacheProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheHelper"/> class based on the provided providers. 
        /// </summary>
        /// <param name="httpCacheProvider">
        /// The http Cache Provider.
        /// </param>
        /// <param name="staticCacheProvider">
        /// The static Cache Provider.
        /// </param>
        /// <param name="requestCacheProvider">
        /// The request Cache Provider.
        /// </param>
	    public CacheHelper(
            IRuntimeCacheProvider httpCacheProvider,
            ICacheProvider staticCacheProvider,
            ICacheProvider requestCacheProvider)
        {
            if (httpCacheProvider == null) throw new ArgumentNullException(nameof(httpCacheProvider));
            if (staticCacheProvider == null) throw new ArgumentNullException(nameof(staticCacheProvider));
            if (requestCacheProvider == null) throw new ArgumentNullException(nameof(requestCacheProvider));

            RuntimeCache = httpCacheProvider;
            StaticCache = staticCacheProvider;
            RequestCache = requestCacheProvider;
        }

        /// <summary>
        /// Gets or sets the current Request cache
        /// </summary>
        public ICacheProvider RequestCache { get; internal set; }

        /// <summary>
        /// Gets or sets the current Runtime cache
        /// </summary>
        public ICacheProvider StaticCache { get; internal set; }

        /// <summary>
        /// Gets or sets the current Runtime cache
        /// </summary>
	    public IRuntimeCacheProvider RuntimeCache { get; internal set; }

        /// <summary>
        /// Creates a cache helper with disabled caches
        /// </summary>
        /// <returns>
        /// The <see cref="CacheHelper"/>
        /// </returns>
        /// <remarks>
        /// Good for unit testing
        /// </remarks>
        public static CacheHelper CreateDisabledCacheHelper()
        {
            return new CacheHelper(NullRuntimeCache, NullStaticCache, NullRequestCache);
        }
    }
}