namespace Merchello.Providers.Payment.PayPal.Services
{
    using System;

    using global::PayPal;

    using Merchello.Providers.Payment.PayPal.Models;

    using Umbraco.Core;

    /// <summary>
    /// A base class of <see cref="IPayPalApiService"/>s.
    /// </summary>
    public class PayPalApiServiceBase : IPayPalApiService
    {

        private readonly PayPalProviderSettings _settings;

        /// <summary>
        /// The <see cref="IPayPalApiPaymentService"/>.
        /// </summary>
        private Lazy<IPayPalApiPaymentService> _payment;


        protected PayPalApiServiceBase(PayPalProviderSettings settings)
        {
            Mandate.ParameterNotNull(settings, "settings");
            _settings = settings;
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

        private void Initialize()
        {

        }
    }
}