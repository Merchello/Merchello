namespace Merchello.Core.Cache
{
    using System;
    using Gateways.Shipping;
    using Models;

    /// <summary>
    /// Merchello cache keys.
    /// </summary>
    internal static class CacheKeys
    {
        /// <summary>
        /// Returns a cache key intended for runtime caching of a <see cref="ICustomerBase"/>
        /// </summary>
        /// <param name="entityKey">
        /// The entity key of the customer
        /// </param>
        /// <returns>
        /// The customer cache key
        /// </returns>
        /// <remarks>
        /// Note the entity key is not the same as the primary key (or key).
        /// This is because of the implementation / mapping between an anonymous customer and and customer
        /// </remarks>
        internal static string CustomerCacheKey(Guid entityKey)
        {
            return string.Format("merchello.customer.{0}", entityKey);   
        }

        /// <summary>
        /// CacheKey for request cache only. Used to check if the customer is logged in.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <remarks>
        /// TODO look at if this can be introduced to the MembershipHelper (Umbraco)
        /// </remarks>
        internal static string CustomerIsLoggedIn(Guid entityKey)
        {
            return string.Format("merchello.customer.isloggedin.{0}", entityKey);
        }

        /// <summary>
        /// CacheKey for request cache only. Used to check if current customer login has been validated against the member.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string EnsureIsLoggedInCustomerValidated(Guid entityKey)
        {
            return string.Format("merchello.customer.ensureIsLoggedIn.{0}", entityKey);
        }

        /// <summary>
        /// Returns a cache key intend for runtime caching of a <see cref="IItemCache"/>
        /// </summary>
        /// <param name="entityKey">
        /// The entity key of the entity associated with the <see cref="IItemCache"/>
        /// </param>
        /// <param name="itemCacheTfKey">The type field key for the cache</param>
        /// <param name="versionKey">The version key for the cache</param>
        /// <returns>
        /// The cache key for an <see cref="IItemCache"/>
        /// </returns>
        internal static string ItemCacheCacheKey(Guid entityKey, Guid itemCacheTfKey, Guid versionKey)
        {
            return string.Format("merchello.itemcache.{0}.{1}.{2}", itemCacheTfKey, entityKey, versionKey);
        }

        /// <summary>
        /// Returns a cache key intended for runtime caching of a <see cref="IShippingGatewayMethod"/>
        /// </summary>
        /// <param name="shipMethodKey">The unique key (GUID) of the <see cref="IShipMethod"/></param>
        /// <returns>
        /// The <see cref="IShipMethod"/> cache key
        /// </returns>
        internal static string GatewayShipMethodCacheKey(Guid shipMethodKey)
        {
            return string.Format("merchello.gatewayshipmethod.{0}", shipMethodKey);
        }

        /// <summary>
        /// Returns a cache key intended for runtime caching of ShippingGateway ship methods
        /// </summary>
        /// <param name="providerKey">The provider key</param>
        /// <returns>
        /// The ship methods cache key
        /// </returns>
        /// <remarks>
        /// TODO RSS - this should be reviewed as it infers that a collection is in cache rather than single entities
        /// </remarks>
        internal static string ShippingGatewayShipMethodsCacheKey(Guid providerKey)
        {
            return string.Format("merchello.shippingateway.shipmethods.{0}", providerKey);
        }

        /// <summary>
        /// Returns a cache key intended for ShippingGatewayProviders rate quotes
        /// </summary>
        /// <param name="shipmentKey">The shipment key</param>
        /// <param name="shipMethodKey">The ship method key</param>
        /// <param name="versionKey">The version key</param>
        /// <returns>
        /// The shipping rate quote cache key
        /// </returns>
        internal static string ShippingGatewayProviderShippingRateQuoteCacheKey(Guid shipmentKey, Guid shipMethodKey, Guid versionKey)
        {
            return string.Format("merchello.shippingratequote.{0}.{1}.{2}", shipmentKey, shipMethodKey, versionKey);
        }

        /// <summary>
        /// Returns a cache key intended for use in repository caching
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity</typeparam>
        /// <param name="key">
        /// The primary key of the entity
        /// </param>
        /// <returns>
        /// The entity cache key.
        /// </returns>
        /// <remarks>
        /// Primarily used in repository caching of entities
        /// </remarks>
        internal static string GetEntityCacheKey<TEntity>(Guid key)
        {
            return string.Format("{0}.{1}", typeof(TEntity).Name, key);
        }

        /// <summary>
        /// Returns the cache key used to store the Umbraco lang file.
        /// </summary>
        /// <param name="lang">
        /// The lang.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string GetLocalizationCacheKey(string lang)
        {
            return string.Format("merch-localize-{0}", string.IsNullOrEmpty(lang) ? "en" : lang);
        }
    }
}