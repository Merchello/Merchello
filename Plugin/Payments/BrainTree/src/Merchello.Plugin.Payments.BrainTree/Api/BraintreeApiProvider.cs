namespace Merchello.Plugin.Payments.Braintree.Api
{
    using System;

    using Merchello.Core;
    using Merchello.Plugin.Payments.Braintree.Models;

    /// <summary>
    /// Represents a <see cref="BraintreeApiProvider"/>.
    /// </summary>
    public class BraintreeApiProvider : IBraintreeApiProvider
    {
        /// <summary>
        /// The <see cref="BraintreeProviderSettings"/>.
        /// </summary>
        private readonly BraintreeProviderSettings _settings;

        /// <summary>
        /// The <see cref="IBraintreeCustomerApiProvider"/>.
        /// </summary>
        private Lazy<IBraintreeCustomerApiProvider> _customer;

        /// <summary>
        /// The <see cref="IBraintreePaymentMethodApiProvider"/>.
        /// </summary>
        private Lazy<IBraintreePaymentMethodApiProvider> _paymentMethod;

        /// <summary>
        /// The <see cref="IBraintreeSubscriptionApiProvider"/>.
        /// </summary>
        private Lazy<IBraintreeSubscriptionApiProvider> _subscription; 

        /// <summary>
        /// The <see cref="IBraintreeTransactionApiProvider"/>.
        /// </summary>
        private Lazy<IBraintreeTransactionApiProvider> _transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeApiProvider"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreeApiProvider(BraintreeProviderSettings settings)
            : this(MerchelloContext.Current, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeApiProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        internal BraintreeApiProvider(IMerchelloContext merchelloContext, BraintreeProviderSettings settings)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(settings, "settings");

            this._settings = settings;

            this.Initialize(merchelloContext);
        }

        /// <summary>
        /// Gets the customer API provider.
        /// </summary>
        public IBraintreeCustomerApiProvider Customer
        {
            get
            {
                return _customer.Value;
            }
        }

        /// <summary>
        /// Gets the payment method API provider.
        /// </summary>
        public IBraintreePaymentMethodApiProvider PaymentMethod
        {
            get
            {
                return _paymentMethod.Value;
            }
        }

        /// <summary>
        /// Gets the subscription API provider.
        /// </summary>
        public IBraintreeSubscriptionApiProvider Subscription
        {
            get
            {
                return _subscription.Value;
            }
        }

        /// <summary>
        /// Gets the transaction API provider.
        /// </summary>
        public IBraintreeTransactionApiProvider Transaction
        {
            get
            {
                return _transaction.Value;
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

            if (_customer == null)
                _customer = new Lazy<IBraintreeCustomerApiProvider>(() => new BraintreeCustomerApiProvider(merchelloContext, _settings));

            if (_paymentMethod == null)
                _paymentMethod = new Lazy<IBraintreePaymentMethodApiProvider>(() => new BraintreePaymentMethodApiProvider(merchelloContext, _settings));

            if (_subscription == null)
                _subscription = new Lazy<IBraintreeSubscriptionApiProvider>(() => new BraintreeSubscriptionApiProvider(merchelloContext, _settings));

            if (_transaction == null)
                _transaction = new Lazy<IBraintreeTransactionApiProvider>(() => new BraintreeTransactionApiProvider(merchelloContext, _settings));
        }
    }
}