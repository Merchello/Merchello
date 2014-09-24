namespace Merchello.Plugin.Payments.Braintree.Api
{
    using System;

    using global::Braintree;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Defines the BraintreeCustomerApiProvider.
    /// </summary>
    public interface IBraintreeCustomerApiProvider
    {
        /// <summary>
        /// Creates a Braintree <see cref="Customer"/> from a Merchello <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The "nonce-from-the-client"
        /// </param>
        /// <param name="billingAddress">
        /// The billing Address.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{Customer}"/>.
        /// </returns>
        Attempt<Customer> Create(ICustomer customer, string paymentMethodNonce = "", IAddress billingAddress = null);

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The "nonce-from-the-client"
        /// </param>
        /// <param name="billingAddress">
        /// The billing Address.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{Customer}"/>.
        /// </returns>
        Attempt<Customer> Update(ICustomer customer, string paymentMethodNonce = "", IAddress billingAddress = null);

        /// <summary>
        /// Deletes the Braintree <see cref="Customer"/> corresponding with the Merchello <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The true of false indicating the delete success.
        /// </returns>
        bool Delete(ICustomer customer);

        /// <summary>
        /// Gets the Braintree <see cref="Customer"/> corresponding to the Merchello <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <param name="createOnNotFound">True or false indicating whether or not the customer should be automatically created if not found</param>
        /// <returns>
        /// The <see cref="Customer"/>.
        /// </returns>
        Customer GetBraintreeCustomer(Guid customerKey, bool createOnNotFound = true);

        /// <summary>
        /// Gets the Braintree <see cref="Customer"/> corresponding to the Merchello <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="createOnNotFound">
        /// True or false indicating whether or not the customer should be automatically created if not found
        /// </param>
        /// <returns>
        /// The <see cref="Customer"/>.
        /// </returns>
        Customer GetBraintreeCustomer(ICustomer customer, bool createOnNotFound = true);

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