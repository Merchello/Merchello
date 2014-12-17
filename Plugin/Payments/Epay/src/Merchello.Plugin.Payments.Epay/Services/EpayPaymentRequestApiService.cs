namespace Merchello.Plugin.Payments.Epay.Services
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Plugin.Payments.Epay.Models;
    using Merchello.Plugin.Payments.Epay.Services.Interfaces;
    
    using Umbraco.Core;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// Represents an EpayPaymentRequestApiService.
    /// </summary>
    internal class EpayPaymentRequestApiService : IEpayPaymentRequestApiService
    {
        /// <summary>
        /// The <see cref="PaymentRequestClient"/>.
        /// </summary>
        private static PaymentRequestClient _client = new PaymentRequestClient();

        public EpayPaymentRequestApiService(EpayProcessorSettings settings)
        {
            
        }

        public EpayPaymentRequestApiService(IMerchelloContext merchelloContext, EpayProcessorSettings settings)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(settings, "settings");

        }

        public void CreatePaymentRequest(string reference, DateTime? exactClosingDate = null, int closeAfterXPayments = 1)
        {
            throw new NotImplementedException();
        }
    }
}