namespace Merchello.Plugin.Payments.Braintree.Caching
{
    using System;

    /// <summary>
    /// The cache keys used in this Braintree plugin
    /// </summary>
    internal static class CacheKeys
    {
        /// <summary>
        /// Cache key to save Braintree customer.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string BraintreeCustomer(Guid customerKey)
        {
            return string.Format("braintree.customerId.{0}", customerKey);
        }
    }
}