namespace Merchello.Plugin.Payments.Braintree.Api
{
    using System;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Plugin.Payments.Braintree.Exceptions;
    using Merchello.Plugin.Payments.Braintree.Models;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents the BraintreePaymentMethodApiProvider.
    /// </summary>
    internal class BraintreePaymentMethodApiProvider : BraintreeApiProviderBase, IBraintreePaymentMethodApiProvider
    {
        /// <summary>
        /// The <see cref="IBraintreeCustomerApiProvider"/>.
        /// </summary>
        private readonly IBraintreeCustomerApiProvider _braintreeCustomerApiProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentMethodApiProvider"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreePaymentMethodApiProvider(BraintreeProviderSettings settings)
            : this(Core.MerchelloContext.Current, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentMethodApiProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreePaymentMethodApiProvider(IMerchelloContext merchelloContext, BraintreeProviderSettings settings)
            : this(merchelloContext, settings, new BraintreeCustomerApiProvider(merchelloContext, settings))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentMethodApiProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="customerApiProvider">
        /// The customer api provider.
        /// </param>
        internal BraintreePaymentMethodApiProvider(IMerchelloContext merchelloContext, BraintreeProviderSettings settings, IBraintreeCustomerApiProvider customerApiProvider)
            : base(merchelloContext, settings)
        {
            Mandate.ParameterNotNull(customerApiProvider, "customerApiProvider");

            _braintreeCustomerApiProvider = customerApiProvider;
        }

        /// <summary>
        /// Adds a credit card to an existing customer.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        /// <param name="billingAddress">
        /// The billing address.
        /// </param>
        /// <param name="isDefault">
        /// A value indicating whether or not this payment method should become the default payment method.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{PaymentMethod}"/> indicating whether the payment method creation was successful.
        /// </returns>
        public Attempt<PaymentMethod> Create(ICustomer customer, string paymentMethodNonce, IAddress billingAddress = null, bool isDefault = true)
        {
            return this.Create(customer, paymentMethodNonce, string.Empty, billingAddress, isDefault);
        }

        /// <summary>
        /// Adds a credit card to an existing customer.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="billingAddress">
        /// The billing address.
        /// </param>
        /// <param name="isDefault">
        /// A value indicating whether or not this payment method should become the default payment method.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{PaymentMethod}"/> indicating whether the payment method creation was successful.
        /// </returns>
        public Attempt<PaymentMethod> Create(ICustomer customer, string paymentMethodNonce, string token, IAddress billingAddress = null, bool isDefault = true)
        {
            //// Asserts the customer exists or creates one in BrainTree if it does not exist
            var btc = _braintreeCustomerApiProvider.GetBraintreeCustomer(customer);

            var request = RequestFactory.CreatePaymentMethodRequest(customer, paymentMethodNonce);

            if (!string.IsNullOrEmpty(token)) request.Token = token;

            if (billingAddress != null) request.BillingAddress = RequestFactory.CreatePaymentMethodAddressRequest(billingAddress);

            var result = BraintreeGateway.PaymentMethod.Create(request);

            if (result.IsSuccess())
            {
                RuntimeCache.ClearCacheItem(this.MakeCustomerCacheKey(customer));

                return Attempt<PaymentMethod>.Succeed(result.Target);
            }

            var error = new BraintreeApiException(result.Errors);

            LogHelper.Error<BraintreeCustomerApiProvider>("Failed to add a credit card to a customer", error);

            return Attempt<PaymentMethod>.Fail(error);
        }

        /// <summary>
        /// Updates an existing payment method.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="billingAddress">
        /// The billing address.
        /// </param>
        /// <param name="updateExisting">
        /// The update existing.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{PaymentProvider}"/> indicating whether the update was successful.
        /// </returns>
        public Attempt<PaymentMethod> Update(string token, IAddress billingAddress, bool updateExisting = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a <see cref="PaymentMethod"/>.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// A value indicating true or false.
        /// </returns>
        public bool Delete(string token)
        {
            if (!this.Exists(token)) return false;

            BraintreeGateway.PaymentMethod.Delete(token);

            return true;
        }

        /// <summary>
        /// Returns true or false indicating whether the customer exists in Braintree.
        /// </summary>
        /// <param name="token">
        /// The token reference.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the payment method exists.
        /// </returns>
        public bool Exists(string token)
        {
            try
            {
                var paymentMethod = (PaymentMethod)RuntimeCache.GetCacheItem(this.MakePaymentMethodCacheKey(token), () => BraintreeGateway.PaymentMethod.Find(token));
                
                return paymentMethod != null;
            }
            catch (Exception)
            {
                RuntimeCache.ClearCacheItem(this.MakePaymentMethodCacheKey(token));
                return false;
            }
        }

        /// <summary>
        /// Gets a <see cref="PaymentMethod"/>.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentMethod"/>.
        /// </returns>
        public PaymentMethod GetPaymentMethod(string token)
        {
            if (this.Exists(token))
            {
                return
                    (PaymentMethod)
                    RuntimeCache.GetCacheItem(
                        this.MakePaymentMethodCacheKey(token),
                        () => BraintreeGateway.PaymentMethod.Find(token));
            }

            return null;
        }

        /// <summary>
        /// Gets the collection of all expired credit cards.
        /// </summary>
        /// <returns>
        /// The <see cref="ResourceCollection{CreditCard}"/>.
        /// </returns>
        public ResourceCollection<CreditCard> GetExpiredCreditCards()
        {
            return BraintreeGateway.CreditCard.Expired();
        }

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
        public ResourceCollection<CreditCard> GetCreditCardsExpiring(DateTime beginRange, DateTime endRange)
        {
            return BraintreeGateway.CreditCard.ExpiringBetween(beginRange, endRange);
        }
    }
}