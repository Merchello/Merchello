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
        /// The <see cref="IPayPalExpressCheckoutService"/>.
        /// </summary>
        private Lazy<IPayPalExpressCheckoutService> _expressCheckout;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalApiService"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public PayPalApiService(PayPalProviderSettings settings)
            : base(settings)
        {
            Ensure.ParameterNotNull(settings, "settings");
            _settings = settings;

            this.Initialize();
        }

        /// <summary>
        /// Gets the <see cref="IPayPalExpressCheckoutService"/>.
        /// </summary>
        public IPayPalExpressCheckoutService ExpressCheckout
        {
            get
            {
                return _expressCheckout.Value;
            }
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        private void Initialize()
        {
            this._expressCheckout = new Lazy<IPayPalExpressCheckoutService>(() => new PayPalExpressCheckoutService(_settings));
        }
    }
}