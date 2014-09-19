namespace Merchello.Plugin.Payments.Braintree.Persistence.Factories
{
    using System;
    using global::Braintree;
    using Core.Models;
    using Models;

    /// <summary>
    /// The <see cref="BraintreeRequestFactory"/>.
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

        public CustomerRequest CreateCustomerRequest(ICustomer customer)
        {
            if (customer == null) throw new ArgumentNullException("customer");

            return new CustomerRequest()
                       {
                           CustomerId = customer.Key.ToString(),
                           FirstName = customer.FirstName,
                           LastName = customer.LastName,
                           Email = customer.Email
                       };
        }

    }    
}