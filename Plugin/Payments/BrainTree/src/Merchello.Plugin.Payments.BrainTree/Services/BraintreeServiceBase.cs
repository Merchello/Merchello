namespace Merchello.Plugin.Payments.Braintree.Services
{
    using global::Braintree;
    using Core;
    using Umbraco.Core.Cache;

    /// <summary>
    /// A base class for local Braintree services.
    /// </summary>
    internal abstract class BraintreeServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeServiceBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="braintreeGateway">
        /// The <see cref="BraintreeGateway"/>.
        /// </param>
        protected BraintreeServiceBase(IMerchelloContext merchelloContext, BraintreeGateway braintreeGateway)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(braintreeGateway, "braintreeGateway");

            MerchelloContext = merchelloContext;

            BraintreeGateway = braintreeGateway;
        }

        /// <summary>
        /// Gets the merchello context.
        /// </summary>
        protected IMerchelloContext MerchelloContext { get; private set; }

        /// <summary>
        /// Gets the braintree gateway.
        /// </summary>
        protected BraintreeGateway BraintreeGateway { get; private set; }

        /// <summary>
        /// Gets the runtime cache.
        /// </summary>
        protected IRuntimeCacheProvider RuntimeCache 
        { 
            get { return MerchelloContext.Cache.RuntimeCache; } 
        }

        /// <summary>
        /// The try get cached.
        /// </summary>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected T TryGetCached<T>(string cacheKey)
        {
            return (T)RuntimeCache.GetCacheItem(cacheKey);
        }
    }
}