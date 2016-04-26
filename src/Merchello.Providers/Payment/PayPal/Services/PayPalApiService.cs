namespace Merchello.Providers.Payment.PayPal.Services
{
    using System;

    using Merchello.Providers.Payment.PayPal.Models;

    using global::PayPal;

    using Umbraco.Core;

    /// <summary>
    /// Represents a PayPal API Service.
    /// </summary>
    public class PayPalApiService : PayPalApiServiceBase, IPayPalApiService
    {
        /// <summary>
        /// The <see cref="PayPalProviderSettings"/>.
        /// </summary>
        private readonly PayPalProviderSettings _settings;

        /// <summary>
        /// The <see cref="IPayPalApiPaymentService"/>.
        /// </summary>
        private Lazy<IPayPalApiPaymentService> _payment;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalApiService"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public PayPalApiService(PayPalProviderSettings settings)
            : base(settings)
        {
            Mandate.ParameterNotNull(settings, "settings");
            _settings = settings;

            this.Initialize();
        }

        /// <summary>
        /// Gets the <see cref="IPayPalApiPaymentService"/>.
        /// </summary>
        public IPayPalApiPaymentService Payment
        {
            get
            {
                return _payment.Value;
            }
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        private void Initialize()
        {
            _payment = new Lazy<IPayPalApiPaymentService>(() => new PayPalApiPaymentService(_settings));
        }
    }
}