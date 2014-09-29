using Umbraco.Core.Events;

namespace Merchello.Plugin.Payments.Braintree.Services
{
    using System;
    using System.Collections.Generic;
    using global::Braintree;
    using Core;
    using Exceptions;
    using Models;
    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents a BraintreeSubscriptionApiProvider.
    /// </summary>
    internal class BraintreeSubscriptionApiService : BraintreeApiServiceBase, IBraintreeSubscriptionApiService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeSubscriptionApiService"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreeSubscriptionApiService(BraintreeProviderSettings settings)
            : this(Core.MerchelloContext.Current, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeSubscriptionApiService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        internal BraintreeSubscriptionApiService(IMerchelloContext merchelloContext, BraintreeProviderSettings settings)
            : base(merchelloContext, settings)
        {
        }

        #region Events

        /// <summary>
        /// Occurs before the Create
        /// </summary>
        public static event TypedEventHandler<BraintreeSubscriptionApiService, Core.Events.NewEventArgs<SubscriptionRequest>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<BraintreeSubscriptionApiService, Core.Events.NewEventArgs<Subscription>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<BraintreeSubscriptionApiService, SaveEventArgs<SubscriptionRequest>> Updating;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<BraintreeSubscriptionApiService, SaveEventArgs<Subscription>> Updated;

        #endregion

        /// <summary>
        /// Creates a <see cref="SubscriptionRequest"/>.
        /// </summary>
        /// <param name="paymentMethodToken">
        /// The payment method token.
        /// </param>
        /// <param name="planId">
        /// The plan id.
        /// </param>
        /// <param name="price">
        /// An optional price used to override the plan price.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{Subscription}"/>.
        /// </returns>
        public Attempt<Subscription> Create(string paymentMethodToken, string planId, decimal? price = null)
        {
            return Create(RequestFactory.CreateSubscriptionRequest(paymentMethodToken, planId, price));           
        }

        /// <summary>
        /// Creates a <see cref="SubscriptionRequest"/>.
        /// </summary>
        /// <param name="paymentMethodToken">
        /// The payment method token.
        /// </param>
        /// <param name="planId">
        /// The plan id.
        /// </param>
        /// <param name="trialDuration">
        /// The trial duration.
        /// </param>
        /// <param name="trialDurationUnit">
        /// The trial duration unit.
        /// </param>
        /// <param name="addTrialPeriod">
        /// The add trial period.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{Subscription}"/>.
        /// </returns>
        public Attempt<Subscription> Create(string paymentMethodToken, string planId, int trialDuration, SubscriptionDurationUnit trialDurationUnit, bool addTrialPeriod = false)
        {
            return Create(RequestFactory.CreateSubscriptionRequest(paymentMethodToken, planId, trialDuration, trialDurationUnit, addTrialPeriod));
        }

        /// <summary>
        /// Creates a <see cref="SubscriptionRequest"/>.
        /// </summary>
        /// <param name="paymentMethodToken">
        /// The payment method token.
        /// </param>
        /// <param name="planId">
        /// The plan id.
        /// </param>
        /// <param name="firstBillingDate">
        /// The first billing date.
        /// </param>
        /// <returns>
        /// The <see cref="SubscriptionRequest"/>.
        /// </returns>
        public Attempt<Subscription> Create(string paymentMethodToken, string planId, DateTime firstBillingDate)
        {
            return Create(RequestFactory.CreateSubscriptionRequest(paymentMethodToken, planId, firstBillingDate));
        }

        /// <summary>
        /// Creates a <see cref="Subscription"/>.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{Subscription}"/>.
        /// </returns>
        public Attempt<Subscription> Create(SubscriptionRequest request)
        {
            var result = BraintreeGateway.Subscription.Create(request);

            Creating.RaiseEvent(new Core.Events.NewEventArgs<SubscriptionRequest>(request), this);

            if (result.IsSuccess())
            {
                return Attempt<Subscription>.Succeed(result.Target);
            }

            var error = new BraintreeApiException(result.Errors, result.Message);

            LogHelper.Error<BraintreeSubscriptionApiService>("Failed to create a subscription", error);

            return Attempt<Subscription>.Fail(error);
        }

        public bool Cancel(string subscriptionId)
        {
            throw new NotImplementedException();
        }

        public Attempt<Subscription> Update(SubscriptionRequest request)
        {
            var result = BraintreeGateway.Subscription.Create(request);

            if (result.IsSuccess())
            {
                return Attempt<Subscription>.Succeed(result.Target);
            }

            var error = new BraintreeApiException(result.Errors, result.Message);

            LogHelper.Error<BraintreeSubscriptionApiService>("Failed to create a subscription", error);

            return Attempt<Subscription>.Fail(error);
        }

        public Subscription GetSubscription(string subscriptionId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Discount> GetAllDiscounts()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<AddOn> GetAllAddOns()
        {
            throw new System.NotImplementedException();
        }

    }
}