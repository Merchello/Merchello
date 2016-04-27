namespace Merchello.Providers.Payment.PayPal.Controllers
{
    using System;
    using System.Net.Http;
    using System.Web.Http;

    using Merchello.Core.Logging;
    using Merchello.Providers.Payment.PayPal.Provider;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The default controller for handling PayPal responses for ExpressCheckout.
    /// </summary>
    [PluginController("MerchelloProviders")]
    public class PayPalExpressApiController : PayPalApiControllerBase
    {
        /// <summary>
        /// The <see cref="PayPalExpressCheckoutPaymentGatewayMethod"/>.
        /// </summary>
        private PayPalExpressCheckoutPaymentGatewayMethod _paymentMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalExpressApiController"/> class.
        /// </summary>
        public PayPalExpressApiController()
        {
            this.Initialize();
        }

        /// <summary>
        /// The success.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="paymentKey">
        /// The payment key.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="payerId">
        /// The payer id.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpGet]
        public override HttpResponseMessage Success(Guid invoiceKey, Guid paymentKey, string token, string payerId)
        {
            MultiLogHelper.Info<PayPalExpressApiController>("Received a success");
            throw new NotImplementedException();
        }

        /// <summary>
        /// The cancel.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="paymentKey">
        /// The payment key.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="payerId">
        /// The payer id.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpGet]
        public override HttpResponseMessage Cancel(Guid invoiceKey, Guid paymentKey, string token, string payerId = null)
        {
            MultiLogHelper.Info<PayPalExpressApiController>("Received a cancel");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        private void Initialize()
        {
            var logData = MultiLogger.GetBaseLoggingData();
            logData.AddCategory("Controllers");
            logData.AddCategory("PayPal");

            var provider = GatewayContext.Payment.GetProviderByKey(Constants.PayPal.GatewayProviderSettingsKey);
            if (provider == null)
            {
                var nullRef =
                    new NullReferenceException(
                        "PayPalPaymentGatewayProvider is not activated or has not been resolved.");
                MultiLogHelper.Error<PayPalExpressApiController>(
                    "Failed to find active PayPalPaymentGatewayProvider.",
                    nullRef,
                    logData);

                throw nullRef;
            }

            _paymentMethod = provider.GetPaymentGatewayMethodByPaymentCode(Constants.PayPal.PaymentCodes.ExpressCheckout) as PayPalExpressCheckoutPaymentGatewayMethod;

            if (_paymentMethod == null)
            {
                var nullRef = new NullReferenceException("PayPalExpressCheckoutPaymentGatewayMethod could not be instantiated");
                MultiLogHelper.Error<PayPalExpressApiController>("PayPalExpressCheckoutPaymentGatewayMethod was null", nullRef, logData);

                throw nullRef;
            }
        }
    }
}