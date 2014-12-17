namespace Merchello.Plugin.Payments.Epay.Services
{
    using System;

    using EPay.API;
    using EPay.API.PaymentRequest;
    using EPay.API.PaymentRequest.CreatePaymentRequest.Request;
    using EPay.API.PaymentWindow;

    using Merchello.Plugin.Payments.Epay.Models;

    using Umbraco.Core;

    /// <summary>
    /// A factory responsible for building Epay request objects.
    /// </summary>
    internal class RequestFactory
    {
        /// <summary>
        /// The <see cref="authentication"/>.
        /// </summary>
        private readonly authentication _authentication;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestFactory"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public RequestFactory(EpayProcessorSettings settings)
        {
            _authentication = settings.ToAuthentication();
        }

        /// <summary>
        /// Builds a <see cref="paymentrequest"/>.
        /// </summary>
        /// <param name="reference">
        /// Reference to the payment request.
        /// </param>
        /// <param name="closeAfterXPayments">
        /// Specify the number of payments to be completed before the payment request closes.
        /// </param>
        /// <param name="exactCloseDate">
        /// Specify a closing date for the payment request.
        /// </param>
        /// <param name="parameters">
        /// Optional parameters to add to the payment request.
        /// </param>
        /// <returns>
        /// The <see cref="paymentrequest"/>.
        /// </returns>
        public paymentrequest BuildPaymentRequest(string reference, int closeAfterXPayments = 1, DateTime? exactCloseDate = null, parameters parameters = null)
        {
            Mandate.ParameterNotNullOrEmpty(reference, "reference");

            return new paymentrequest()
                       {
                           reference = reference,
                           closeafterxpayments = closeAfterXPayments,
                           exactclosedate = exactCloseDate,
                           parameters = parameters,
                       };
        }

        /// <summary>
        /// The build create payment request request.
        /// </summary>
        /// <param name="paymentRequest">
        /// The payment request.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <param name="localTimeZone">
        /// The local time zone.
        /// </param>
        /// <returns>
        /// The <see cref="createpaymentrequestrequest"/>.
        /// </returns>
        public createpaymentrequestrequest BuildCreatePaymentRequestRequest(paymentrequest paymentRequest, string language = "en", bool localTimeZone = true)
        {           
            return new createpaymentrequestrequest()
                       {
                           authentication = _authentication,
                           language = language,
                           localtimezone = localTimeZone, 
                           paymentrequest = paymentRequest
                       };
        }
    }
}