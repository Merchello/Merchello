namespace Merchello.Web.Services
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Logging;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Services;

    /// <summary>
    /// A resolver to ensure there is one instance of a proxy service per request.
    /// </summary>
    //// REFACTOR remove this class in V3 and use container with request lifeftime scope
    internal class ProxyEntityServiceResolver : IProxyEntityServiceResolver
    {
        /// <summary>
        /// The _cache.
        /// </summary>
        private readonly ICacheProvider _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyEntityServiceResolver"/> class.
        /// </summary>
        /// <param name="cache">
        /// The <see cref="CacheHelper"/>.
        /// </param>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal ProxyEntityServiceResolver(ICacheProvider cache)
        {
            Mandate.ParameterNotNull(cache, "cache");

            this._cache = cache;
        }


        /// <summary>
        /// Gets or sets the current singleton instance.
        /// </summary>
        public static ProxyEntityServiceResolver Current { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether current has been set.
        /// </summary>
        public static bool HasCurrent
        {
            get
            {
                return Current != null;
            }
        }

        /// <summary>
        /// Creates an instance of a service.
        /// </summary>
        /// <typeparam name="TService">
        /// The type of the service to be returned
        /// </typeparam>
        /// <returns>
        /// The service.
        /// </returns>
        public TService Instance<TService>()
            where TService : IEntityProxyService, new()
        {
            return
                (TService)
                this._cache.GetCacheItem(
                    GetCacheKey(typeof(TService)),
                    () =>
                        {
                            try
                            {
                                var service = (TService)Activator.CreateInstance(typeof(TService));
                                return service;
                            }
                            catch (Exception ex)
                            {
                                var logData = this.GetLoggerData<TService>();
                                MultiLogHelper.Error<ProxyEntityServiceResolver>("Failed to instantiate service", ex, logData);
                                throw;
                            }
                        });
        }

        /// <summary>
        /// Creates an instance of a service with arguments
        /// </summary>
        /// <param name="constructorArgumentValues">
        /// The constructor argument values.
        /// </param>
        /// <typeparam name="TService">
        /// The type of the service to resolve
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if service cannot be resolved
        /// </exception>
        public TService Instance<TService>(object[] constructorArgumentValues)
            where TService : class, IEntityProxyService
        {
            //// We may need to manage separate instances of these services if the constructor arguments are different
            //// so that we can assert the values returned are what we expect.
            var suffix = string.Join(".", constructorArgumentValues.Select(x => x.ToString()));
            var cacheKey = GetCacheKey(typeof(TService), suffix);

            return (TService)this._cache.GetCacheItem(
                cacheKey,
                () =>
                ActivatorHelper.CreateInstance<TService>(constructorArgumentValues));
        }

        /// <summary>
        /// Gets the request cache key.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="suffix">
        /// The cache key suffix
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetCacheKey(Type service, string suffix = "")
        {
            return string.Format("{0}.{1}{2}", typeof(ProxyEntityServiceResolver), service, suffix);
        }

        /// <summary>
        /// Creates the <see cref="IExtendedLoggerData"/>.
        /// </summary>
        /// <param name="constructorArgs">
        /// The constructor args.
        /// </param>
        /// <typeparam name="TService">
        /// The type of the service attempting to be instantiated
        /// </typeparam>
        /// <returns>
        /// The <see cref="IExtendedLoggerData"/>.
        /// </returns>
        private IExtendedLoggerData GetLoggerData<TService>(object[] constructorArgs = null)
        {
            var loggerData = MultiLogger.GetBaseLoggingData();
            loggerData.AddCategory("Resolvers");
            loggerData.AddCategory("Services");
            
            return loggerData;
        }
    }
}
