﻿namespace Merchello.Providers.Payment.Braintree.Services
{
    using System;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Providers.Models;
    using Merchello.Providers.Payment.Braintree.Models;
    using Merchello.Providers.Payment.Exceptions;
    using Merchello.Providers.Payment.Models;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    using PaymentMethod = global::Braintree.PaymentMethod;

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

            this._braintreeCustomerApiService = customerApiService;
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
            var btc = this._braintreeCustomerApiService.GetBraintreeCustomer(customer);

            var request = this.RequestFactory.CreatePaymentMethodRequest(customer, paymentMethodNonce);

            if (!string.IsNullOrEmpty(token)) request.Token = token;

            if (billingAddress != null) request.BillingAddress = this.RequestFactory.CreatePaymentMethodAddressRequest(billingAddress);


            Creating.RaiseEvent(new Core.Events.NewEventArgs<PaymentMethodRequest>(request), this);

            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.PaymentMethod.Create(request));

            if (!attempt.Success) return Attempt<PaymentMethod>.Fail(attempt.Exception);

            var result = attempt.Result;

            if (result.IsSuccess())
            {
                var cacheKey = this.MakePaymentMethodCacheKey(token);

                this.RuntimeCache.ClearCacheItem(cacheKey);

                Created.RaiseEvent(new Core.Events.NewEventArgs<PaymentMethod>(result.Target), this);

                return Attempt<PaymentMethod>.Succeed((PaymentMethod)this.RuntimeCache.GetCacheItem(cacheKey, () => result.Target));
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
            var request = this.RequestFactory.CreatePaymentMethodRequest(billingAddress, updateExisting);
            return this.Update(token, request);
        }

        /// <summary>
        /// Updates a payment method
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        /// <param name="billingAddress">
        /// The billing address.
        /// </param>
        /// <param name="updateExisting">
        /// The update existing.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public Attempt<PaymentMethod> Update(string token, string paymentMethodNonce, IAddress billingAddress, bool updateExisting = true)
        {
            var request = this.RequestFactory.CreatePaymentMethodRequest(billingAddress, updateExisting);
            request.PaymentMethodNonce = paymentMethodNonce;

            return this.Update(token, request);
        }

        /// <summary>
        /// Updates a payment method
        /// </summary>
        /// <param name="token">The payment method token</param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public Attempt<PaymentMethod> Update(string token, PaymentMethodRequest request)
        {
            Mandate.ParameterNotNull(request, "request");

            Updating.RaiseEvent(new SaveEventArgs<PaymentMethodRequest>(request), this);

            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.PaymentMethod.Update(token, request));

            if (!attempt.Success) return Attempt<PaymentMethod>.Fail(attempt.Exception);

            var result = attempt.Result;

            if (result.IsSuccess())
            {
                var cacheKey = this.MakePaymentMethodCacheKey(token);

                this.RuntimeCache.ClearCacheItem(cacheKey);

                Updated.RaiseEvent(new SaveEventArgs<PaymentMethod>(result.Target), this);

                return Attempt<PaymentMethod>.Succeed((PaymentMethod)this.RuntimeCache.GetCacheItem(cacheKey, () => result.Target));
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

            try
            {
                this.BraintreeGateway.PaymentMethod.Delete(token);
                this.RuntimeCache.ClearCacheItem(this.MakePaymentMethodCacheKey(token));
            }
            catch (Exception ex)
            {
                LogHelper.Error<BraintreePaymentMethodApiService>("Braintree API payment method delete request failed.", ex);
                return false;
            }
            
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

            var cacheKey = this.MakePaymentMethodCacheKey(token);

            var paymentMethod = (PaymentMethod) this.RuntimeCache.GetCacheItem(cacheKey);

            if (paymentMethod == null)
            {
                var attempt = this.TryGetApiResult(() => this.BraintreeGateway.PaymentMethod.Find(token));

                if (!attempt.Success) return false;
                paymentMethod = attempt.Result;
                this.RuntimeCache.GetCacheItem(cacheKey, () => paymentMethod);

                return true;
            }

            return true;
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
            var cacheKey = this.MakePaymentMethodCacheKey(token);

            if (this.Exists(token))
            {
                var paymentMethod = (PaymentMethod) this.RuntimeCache.GetCacheItem(cacheKey);

                if (paymentMethod != null) return paymentMethod;

                // this following should never happen as the Exists should cache it but its a good fallback
                var attempt = this.TryGetApiResult(() => this.BraintreeGateway.PaymentMethod.Find(token));

                if (attempt.Success)
                {
                    this.RuntimeCache.GetCacheItem(cacheKey, () => attempt.Result);
                }

                return attempt.Success ? attempt.Result : null;
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
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.CreditCard.Expired());
            return attempt.Success ? attempt.Result : null;
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
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.CreditCard.ExpiringBetween(beginRange, endRange));
            return attempt.Success ? attempt.Result : null;
        }
    }
}