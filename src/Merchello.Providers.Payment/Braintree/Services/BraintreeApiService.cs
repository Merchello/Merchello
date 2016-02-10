namespace Merchello.Providers.Payment.Braintree.Services
{
    using System;

    using Merchello.Core;
    using Merchello.Providers.Payment.Braintree.Models;

    using Umbraco.Core;

    /// <summary>
    /// Represents a <see cref="BraintreeApiService"/>.
    /// </summary>
    public class BraintreeApiService : IBraintreeApiService
    {
        /// <summary>
        /// The <see cref="BraintreeProviderSettings"/>.
        /// </summary>
        private readonly BraintreeProviderSettings _settings;

        /// <summary>
        /// The <see cref="IBraintreeCustomerApiService"/>.
        /// </summary>
        private Lazy<IBraintreeCustomerApiService> _customer;

        /// <summary>
        /// The <see cref="IBraintreePaymentMethodApiService"/>.
        /// </summary>
        private Lazy<IBraintreePaymentMethodApiService> _paymentMethod;

        /// <summary>
        /// The <see cref="IBraintreeSubscriptionApiService"/>.
        /// </summary>
        private Lazy<IBraintreeSubscriptionApiService> _subscription; 

        /// <summary>
        /// The <see cref="IBraintreeTransactionApiService"/>.
        /// </summary>
        private Lazy<IBraintreeTransactionApiService> _transaction;

        /// <summary>
        /// The <see cref="IBraintreeWebhooksApiService"/>.
        /// </summary>
        private Lazy<IBraintreeWebhooksApiService> _webhooks; 

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeApiService"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreeApiService(BraintreeProviderSettings settings)
            : this(MerchelloContext.Current, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeApiService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal BraintreeApiService(IMerchelloContext merchelloContext, BraintreeProviderSettings settings)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(settings, "settings");

            this._settings = settings;

            this.Initialize(merchelloContext);
        }

        /// <summary>
        /// Gets the customer API provider.
        /// </summary>
        public IBraintreeCustomerApiService Customer
        {
            get
            {
                return this._customer.Value;
            }
        }

        /// <summary>
        /// Gets the payment method API provider.
        /// </summary>
        public IBraintreePaymentMethodApiService PaymentMethod
        {
            get
            {
                return this._paymentMethod.Value;
            }
        }

        /// <summary>
        /// Gets the subscription API provider.
        /// </summary>
        public IBraintreeSubscriptionApiService Subscription
        {
            get
            {
                return this._subscription.Value;
            }
        }

        /// <summary>
        /// Gets the transaction API provider.
        /// </summary>
        public IBraintreeTransactionApiService Transaction
        {
            get
            {
                return this._transaction.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="IBraintreeWebhooksApiService"/>.
        /// </summary>
        public IBraintreeWebhooksApiService Webhook 
        {
            get
            {
                return this._webhooks.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="BraintreeProviderSettings"/>.
        /// </summary>
        public BraintreeProviderSettings BraintreeProviderSettings
        {
            get
            {
                return this._settings;
            }
        }

        /// <summary>
        /// Initializes the provider
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        private void Initialize(IMerchelloContext merchelloContext)
        {
            if (this._customer == null)
                this._customer = new Lazy<IBraintreeCustomerApiService>(() => new BraintreeCustomerApiService(merchelloContext, this._settings));

            if (this._paymentMethod == null)
                this._paymentMethod = new Lazy<IBraintreePaymentMethodApiService>(() => new BraintreePaymentMethodApiService(merchelloContext, this._settings, this._customer.Value));

            if (this._subscription == null)
                this._subscription = new Lazy<IBraintreeSubscriptionApiService>(() => new BraintreeSubscriptionApiService(merchelloContext, this._settings));

            if (this._transaction == null)
                this._transaction = new Lazy<IBraintreeTransactionApiService>(() => new BraintreeTransactionApiService(merchelloContext, this._settings));

            if (this._webhooks == null)
                this._webhooks = new Lazy<IBraintreeWebhooksApiService>(() => new BraintreeWebhooksApiService(merchelloContext, this._settings));
        }
    }
}