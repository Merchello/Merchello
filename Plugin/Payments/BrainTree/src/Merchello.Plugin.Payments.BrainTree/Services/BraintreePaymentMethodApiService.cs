using Umbraco.Core.Events;

namespace Merchello.Plugin.Payments.Braintree.Services
{
    using System;
    using global::Braintree;
    using Core;
    using Core.Models;
    using Exceptions;
    using Models;
    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents the BraintreePaymentMethodApiProvider.
    /// </summary>
    internal class BraintreePaymentMethodApiService : BraintreeApiServiceBase, IBraintreePaymentMethodApiService
    {
        /// <summary>
        /// The <see cref="IBraintreeCustomerApiService"/>.
        /// </summary>
        private readonly IBraintreeCustomerApiService _braintreeCustomerApiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentMethodApiService"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreePaymentMethodApiService(BraintreeProviderSettings settings)
            : this(Core.MerchelloContext.Current, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentMethodApiService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreePaymentMethodApiService(IMerchelloContext merchelloContext, BraintreeProviderSettings settings)
            : this(merchelloContext, settings, new BraintreeCustomerApiService(merchelloContext, settings))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentMethodApiService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="customerApiService">
        /// The customer api provider.
        /// </param>
        internal BraintreePaymentMethodApiService(IMerchelloContext merchelloContext, BraintreeProviderSettings settings, IBraintreeCustomerApiService customerApiService)
            : base(merchelloContext, settings)
        {
            Mandate.ParameterNotNull(customerApiService, "customerApiProvider");

            _braintreeCustomerApiService = customerApiService;
        }

        #region Events

        /// <summary>
        /// Occurs before the Create
        /// </summary>
        public static event TypedEventHandler<BraintreePaymentMethodApiService, Core.Events.NewEventArgs<PaymentMethodRequest>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<BraintreePaymentMethodApiService, Core.Events.NewEventArgs<PaymentMethod>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<BraintreePaymentMethodApiService, SaveEventArgs<PaymentMethodRequest>> Updating;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<BraintreePaymentMethodApiService, SaveEventArgs<PaymentMethod>> Updated;

        #endregion

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
            var btc = _braintreeCustomerApiService.GetBraintreeCustomer(customer);

            var request = RequestFactory.CreatePaymentMethodRequest(customer, paymentMethodNonce);

            if (!string.IsNullOrEmpty(token)) request.Token = token;

            if (billingAddress != null) request.BillingAddress = RequestFactory.CreatePaymentMethodAddressRequest(billingAddress);


            Creating.RaiseEvent(new Core.Events.NewEventArgs<PaymentMethodRequest>(request), this);

            var result = BraintreeGateway.PaymentMethod.Create(request);

            if (result.IsSuccess())
            {
                var cacheKey = MakePaymentMethodCacheKey(token);

                RuntimeCache.ClearCacheItem(cacheKey);

                Created.RaiseEvent(new Core.Events.NewEventArgs<PaymentMethod>(result.Target), this);

                return Attempt<PaymentMethod>.Succeed((PaymentMethod)RuntimeCache.GetCacheItem(cacheKey, () => result.Target));
            }

            var error = new BraintreeApiException(result.Errors, result.Message);

            LogHelper.Error<BraintreeCustomerApiService>("Failed to add a credit card to a customer", error);

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
            var request = RequestFactory.CreatePaymentMethodRequest(billingAddress, updateExisting);


            Updating.RaiseEvent(new SaveEventArgs<PaymentMethodRequest>(request), this);

            var result = BraintreeGateway.PaymentMethod.Update(token, request);

            if (result.IsSuccess())
            {
                var cacheKey = MakePaymentMethodCacheKey(token);
                
                RuntimeCache.ClearCacheItem(cacheKey);

                Updated.RaiseEvent(new SaveEventArgs<PaymentMethod>(result.Target), this);

                return Attempt<PaymentMethod>.Succeed((PaymentMethod)RuntimeCache.GetCacheItem(cacheKey, () => result.Target));
            }

            var error = new BraintreeApiException(result.Errors, result.Message);

            LogHelper.Error<BraintreePaymentMethodApiService>("Failed to update payment method", error);

            return Attempt<PaymentMethod>.Fail(error);
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