namespace Merchello.Plugin.Payments.Braintree.Caching
{
    using System;

    /// <summary>
    /// The cache keys used in this Braintree plugin
    /// </summary>
    internal static class CacheKeys
    {
        /// <summary>
        /// Cache key to cache a Braintree customer.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> cache key.
        /// </returns>
        public static string BraintreeCustomer(Guid customerKey)
        {
            return string.Format("braintree.customerId.{0}", customerKey);
        }

        /// <summary>
        /// Cache key to cache a payment method.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> cache key.
        /// </returns>
        public static string BraintreePaymentMethod(string token)
        {
            return string.Format("braintree.paymentmethod.{0}", token);
        }

        /// <summary>
        /// Cache key used to cache a Braintree subscription.
        /// </summary>
        /// <param name="subscriptionId">
        /// The subscription id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> cache key.
        /// </returns>
        public static string BraintreeSubscription(string subscriptionId)
        {
            return string.Format("braintree.subscription.{0}", subscriptionId);
        }
    }
}