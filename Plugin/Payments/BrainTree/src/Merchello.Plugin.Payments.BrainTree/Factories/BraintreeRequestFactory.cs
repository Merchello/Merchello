namespace Merchello.Plugin.Payments.Braintree.Factories
{
    using System;

    using global::Braintree;

    using Merchello.Core.Models;

    /// <summary>
    /// The request factory.
    /// </summary>
    internal class BraintreeRequestFactory
    {
        /// <summary>
        /// Creates a <see cref="ClientTokenRequest"/>.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The <see cref="ClientTokenRequest"/>.
        /// </returns>
        public ClientTokenRequest CreateClientTokenRequest(Guid customerKey)
        {
            return customerKey == Guid.Empty ? 
                new ClientTokenRequest() : 
                new ClientTokenRequest()
                    {
                        CustomerId = customerKey.ToString() 
                    };
        }

    }
}