namespace Merchello.Plugin.Payments.Braintree.Services
{
    using System;

    using global::Braintree;

    using Merchello.Core.Models;

    public interface IBraintreeCustomerService
    {
        /// <summary>
        /// Creates a Braintree <see cref="Customer"/> from a Merchello <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="includeCustomerAddresses">A value indicating whether or not to include the customer addresses in the creation</param>
        /// <returns>
        /// The <see cref="Customer"/>.
        /// </returns>
        Customer Create(ICustomer customer, bool includeCustomerAddresses = true);

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="includeCustomerAddresses">A value indicating whether or not to include the customer addresses in the update</param>
        /// <returns>
        /// The <see cref="Customer"/>.
        /// </returns>
        Customer Update(ICustomer customer, bool includeCustomerAddresses = true);

        /// <summary>
        /// Deletes the Braintree <see cref="Customer"/> corresponding with the Merchello <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        void Delete(ICustomer customer);

        /// <summary>
        /// Gets the Braintree <see cref="Customer"/> corresponding to the Merchello <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The <see cref="Customer"/>.
        /// </returns>
        Customer GetBraintreeCustomer(Guid customerKey);

        /// <summary>
        /// Gets the Braintree <see cref="Customer"/> corresponding to the Merchello <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="Customer"/>.
        /// </returns>
        Customer GetBraintreeCustomer(ICustomer customer);

        /// <summary>
        /// Generates a ClientRequestToken for an anonymous customer
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GenerateClientRequestToken();

        /// <summary>
        /// Generates a ClientRequestToken for a persisted <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GenerateClientRequestToken(ICustomer customer);

        /// <summary>
        /// Returns true or false indicating whether the customer exists in Braintree
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Exists(ICustomer customer);
    }   
}