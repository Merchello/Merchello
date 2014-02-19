using System;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Shipping.RateTable;
using Merchello.Core.Models;

namespace Merchello.Core.Cache
{
    internal static class CacheKeys
    {
        /// <summary>
        /// Returns a cache key intended for runtime caching of a <see cref="ICustomerBase"/>
        /// </summary>
        /// <param name="entityKey"></param>
        /// <returns></returns>
        internal static string CostumerCacheKey(Guid entityKey)
        {
            return string.Format("merchello.customer.{0}", entityKey);   
        }

        /// <summary>
        /// Returns a cache key intend for runtime caching of a <see cref="IItemCache"/>
        /// </summary>
        /// <param name="entityKey"></param>
        /// <param name="itemCacheTfKey">The type field key for the cache</param>
        /// <param name="versionKey">The version key for the cache</param>
        /// <returns></returns>
        internal static string ItemCacheCacheKey(Guid entityKey, Guid itemCacheTfKey, Guid versionKey)
        {
            return string.Format("merchello.itemcache.{0}.{1}.{2}", itemCacheTfKey, entityKey, versionKey);
        }

        /// <summary>
        /// Returns a cache key intended for runtime caching of a <see cref="IGatewayShipMethod"/>
        /// </summary>
        /// <param name="shipMethodKey">The unique key (Guid) of the <see cref="IShipRateTable"/> associated with the rate table</param>
        /// <returns></returns>
        internal static string GatewayShipMethodCacheKey(Guid shipMethodKey)
        {
            return string.Format("merchello.gatewayshipmethod.{0}", shipMethodKey);
        }

        /// <summary>
        /// Returns a cache key intended for runtime caching of ShippingGateway ship methods
        /// </summary>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        internal static string ShippingGatewayShipMethodsCacheKey(Guid providerKey)
        {
            return string.Format("merchello.shippingateway.shipmethods.{0}", providerKey);
        }

        /// <summary>
        /// Returns a cache key intended for ShippingGatewayProviders rate quotes
        /// </summary>
        /// <param name="shipmentKey"></param>
        /// <param name="shipMethodKey"></param>
        /// <param name="versionKey">The </param>
        /// <returns></returns>
        internal static string ShippingGatewayProviderShippingRateQuoteCacheKey(Guid shipmentKey, Guid shipMethodKey, Guid versionKey)
        {
            return string.Format("merchello.shippingratequote.{0}.{1}.{2}", shipmentKey, shipMethodKey,versionKey);
        }

        /// <summary>
        /// Returns a cache key intened for use in repository caching
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static string GetEntityCacheKey<TEntity>(Guid key)
        {
            return string.Format("{0}-{1}", typeof (TEntity).Name, key.ToString());
        }
       
        
        
    }
}