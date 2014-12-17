namespace Merchello.Plugin.Payments.Epay.Services
{
    using System;

    using Merchello.Core;
    using Merchello.Plugin.Payments.Epay.Models;
    using Merchello.Plugin.Payments.Epay.Services.Interfaces;

    using Umbraco.Core;

    /// <summary>
    /// A service that wraps the Epay REST API.
    /// </summary>
    public class EpayApiService
    {
        /// <summary>
        /// The <see cref="EpayProcessorSettings"/>.
        /// </summary>
        private readonly EpayProcessorSettings _settings;

        /// <summary>
        /// The <see cref="EpayPaymentRequestApiService"/>.
        /// </summary>
        private Lazy<EpayPaymentRequestApiService> _paymentRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="EpayApiService"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public EpayApiService(EpayProcessorSettings settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EpayApiService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public EpayApiService(IMerchelloContext merchelloContext, EpayProcessorSettings settings)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(settings, "settings");

            _settings = settings;
            this.Initialize(merchelloContext);
        }

        /// <summary>
        /// Gets the <see cref="IEpayPaymentRequestApiService"/>.
        /// </summary>
        public IEpayPaymentRequestApiService PaymentRequest
        {
            get
            {
                return _paymentRequest.Value;
            }
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        private void Initialize(IMerchelloContext merchelloContext)
        {
            if (_paymentRequest == null) 
                _paymentRequest = new Lazy<EpayPaymentRequestApiService>(() => new EpayPaymentRequestApiService(merchelloContext, _settings));
        }
    }
}