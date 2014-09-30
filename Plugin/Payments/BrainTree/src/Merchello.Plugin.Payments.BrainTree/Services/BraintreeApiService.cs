namespace Merchello.Plugin.Payments.Braintree.Services
{
    using System;
    using Core;
    using Models;

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
                return _customer.Value;
            }
        }

        /// <summary>
        /// Gets the payment method API provider.
        /// </summary>
        public IBraintreePaymentMethodApiService PaymentMethod
        {
            get
            {
                return _paymentMethod.Value;
            }
        }

        /// <summary>
        /// Gets the subscription API provider.
        /// </summary>
        public IBraintreeSubscriptionApiService Subscription
        {
            get
            {
                return _subscription.Value;
            }
        }

        /// <summary>
        /// Gets the transaction API provider.
        /// </summary>
        public IBraintreeTransactionApiService Transaction
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
                _customer = new Lazy<IBraintreeCustomerApiService>(() => new BraintreeCustomerApiService(merchelloContext, _settings));

            if (_paymentMethod == null)
                _paymentMethod = new Lazy<IBraintreePaymentMethodApiService>(() => new BraintreePaymentMethodApiService(merchelloContext, _settings, _customer.Value));

            if (_subscription == null)
                _subscription = new Lazy<IBraintreeSubscriptionApiService>(() => new BraintreeSubscriptionApiService(merchelloContext, _settings));

            if (_transaction == null)
                _transaction = new Lazy<IBraintreeTransactionApiService>(() => new BraintreeTransactionApiService(merchelloContext, _settings));
        }
    }
}