namespace Merchello.Plugin.Payments.Braintree.Services
{
    using System;
    using global::Braintree;
    using Core;
    using Core.Models;
    using Models;
    using Umbraco.Core.Cache;

    /// <summary>
    /// A base class for local Braintree services.
    /// </summary>
    internal abstract class BraintreeApiServiceBase
    {
        /// <summary>
        /// The <see cref="BraintreeApiRequestFactory"/>.
        /// </summary>
        private Lazy<BraintreeApiRequestFactory> _requestFactory;  


        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeApiServiceBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        protected BraintreeApiServiceBase(IMerchelloContext merchelloContext, BraintreeProviderSettings settings)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(settings, "settings");

            MerchelloContext = merchelloContext;

            BraintreeGateway = settings.AsBraintreeGateway();

            Initialize(settings);
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
            get { return this.MerchelloContext.Cache.RuntimeCache; } 
        }

        /// <summary>
        /// Gets the request factory.
        /// </summary>
        protected BraintreeApiRequestFactory RequestFactory
        {
            get
            {
                return _requestFactory.Value;
            }
        }

        /// <summary>
        /// The try get cached.
        /// </summary>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <typeparam name="T">
        /// The type of the cached item to be returned
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected T TryGetCached<T>(string cacheKey)
        {
            return (T)this.RuntimeCache.GetCacheItem(cacheKey);
        }

        /// <summary>
        /// Makes a customer cache key.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> cache key.
        /// </returns>
        protected string MakeCustomerCacheKey(ICustomer customer)
        {
            return Caching.CacheKeys.BraintreeCustomer(customer.Key);
        }

        /// <summary>
        /// Makes a payment method cache key.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> cache key.
        /// </returns>
        protected string MakePaymentMethodCacheKey(string token)
        {
            return Caching.CacheKeys.BraintreePaymentMethod(token);
        }

        /// <summary>
        /// Makes a subscription cache key.
        /// </summary>
        /// <param name="subscriptionId">
        /// The subscription id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> cache key.
        /// </returns>
        protected string MakeSubscriptionCacheKey(string subscriptionId)
        {
            return Caching.CacheKeys.BraintreeSubscription(subscriptionId);
        }

        /// <summary>
        /// Performs class initialization logic.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        private void Initialize(BraintreeProviderSettings settings)
        {
            _requestFactory = new Lazy<BraintreeApiRequestFactory>(() => new BraintreeApiRequestFactory(settings));
        }
    }
}