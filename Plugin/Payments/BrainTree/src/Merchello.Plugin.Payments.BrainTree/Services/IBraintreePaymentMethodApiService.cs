namespace Merchello.Plugin.Payments.Braintree.Services
{
    using System;
    using global::Braintree;
    using Core.Models;
    using Umbraco.Core;

    /// <summary>
    /// Defines the BraintreePaymentMethodApiProvider.
    /// </summary>
    public interface IBraintreePaymentMethodApiService
    {
        /// <summary>
        /// Adds a payment method to a customer.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        /// <param name="billingAddress">The billing address</param>
        /// <param name="isDefault">
        /// A value indicating whether or not this payment method should become the default payment method.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{PaymentMethod}"/> indicating whether the payment method creation was successful.
        /// </returns>
        Attempt<PaymentMethod> Create(ICustomer customer, string paymentMethodNonce, IAddress billingAddress = null, bool isDefault = true);

        /// <summary>
        /// Adds a payment method to a customer.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        /// <param name="token">
        /// A local token to reference the payment method. 
        /// </param>
        /// <param name="billingAddress">
        /// The billing address.
        /// </param>
        /// <param name="isDefault">
        /// A value indicating whether or not this payment method should become the default payment method
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{PaymentMethod}"/> indicating whether the payment method creation was successful.
        /// </returns>
        Attempt<PaymentMethod> Create(ICustomer customer, string paymentMethodNonce, string token, IAddress billingAddress = null, bool isDefault = true);

        /// <summary>
        /// Updates a payment method
        /// </summary>
        /// <param name="token">
        /// The payment method token
        /// </param>
        /// <param name="billingAddress">
        /// The billing address.
        /// </param>
        /// <param name="updateExisting">
        /// If false a new address will be created
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{PaymentMethod}"/> on a successful update.
        /// </returns>
        Attempt<PaymentMethod> Update(string token, IAddress billingAddress, bool updateExisting = true);

        /// <summary>
        /// Deletes a payment method.
        /// </summary>
        /// <param name="token">
        /// The token reference.
        /// </param>
        /// <returns>
        /// Returns true on a successful delete.
        /// </returns>
        bool Delete(string token);

        /// <summary>
        /// Checks for the existence of a payment method associated with the token
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// Returns true if a payment method exists with the token.
        /// </returns>
        bool Exists(string token);

        /// <summary>
        /// Gets a <see cref="PaymentMethod"/>.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        PaymentMethod GetPaymentMethod(string token);

        /// <summary>
        /// Gets the collection of all expired credit cards.
        /// </summary>
        /// <returns>
        /// The <see cref="ResourceCollection{CreditCard}"/>.
        /// </returns>
        ResourceCollection<CreditCard> GetExpiredCreditCards();

        /// <summary>
        /// Gets a collection of credit cards expiring between the date range.
        /// </summary>
        /// <param name="beginRange">
        /// The begin range.
        /// </param>
        /// <param name="endRange">
        /// The end range.
        /// </param>
        /// <returns>
        /// The <see cref="ResourceCollection{CreditCard}"/>.
        /// </returns>
        ResourceCollection<CreditCard> GetCreditCardsExpiring(DateTime beginRange, DateTime endRange);
    }
}