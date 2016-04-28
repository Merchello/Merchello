namespace Merchello.Providers.Payment.PayPal.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Mvc;

    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Providers.Payment.PayPal.Provider;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    using Constants = Merchello.Providers.Constants;

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
        [System.Web.Http.HttpGet]
        public override HttpResponseMessage Success(Guid invoiceKey, Guid paymentKey, string token, string payerId)
        {
            try
            {
                Mandate.ParameterCondition(!Guid.Empty.Equals(invoiceKey), "invoiceKey");
                Mandate.ParameterCondition(!Guid.Empty.Equals(paymentKey), "paymentKey");

                var invoice = InvoiceService.GetByKey(invoiceKey);
                if (invoice == null) throw new NullReferenceException("Invoice was not found.");

                var payment = PaymentService.GetByKey(paymentKey);
                if (payment == null) throw new NullReferenceException("Payment was not found.");

                invoice.CapturePayment(payment, _paymentMethod, invoice.Total);

                // redirect to Site
                CustomerContext.SetValue("invoiceKey", invoiceKey.ToString());
               // var returnUrl = payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.ReturnUrl);
                var response = Request.CreateResponse(HttpStatusCode.Moved);
               // response.Headers.Location = new Uri(returnUrl.Replace("%INVOICE%", invoice.Key.ToString().EncryptWithMachineKey()));
                return response;

            }
            catch (Exception ex)
            {
                var logData = GetExtendedLoggerData();

                var extra = new
                                {
                                    InvoiceKey = invoiceKey, 
                                    PaymentKey = paymentKey,
                                    Token = token,
                                    PayerId = payerId
                                };

                logData.SetValue<object>("extra", extra);

                MultiLogHelper.Error<PayPalExpressApiController>(
                    "Failed to Capture SUCCESSFUL PayPal Express checkout response.",
                    ex,
                    logData);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, extra);
            }
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
        [System.Web.Http.HttpGet]
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
            var provider = GatewayContext.Payment.GetProviderByKey(Constants.PayPal.GatewayProviderSettingsKey);
            if (provider == null)
            {
                var nullRef =
                    new NullReferenceException(
                        "PayPalPaymentGatewayProvider is not activated or has not been resolved.");
                MultiLogHelper.Error<PayPalExpressApiController>(
                    "Failed to find active PayPalPaymentGatewayProvider.",
                    nullRef,
                    GetExtendedLoggerData());

                throw nullRef;
            }

            _paymentMethod = provider.GetPaymentGatewayMethodByPaymentCode(Constants.PayPal.PaymentCodes.ExpressCheckout) as PayPalExpressCheckoutPaymentGatewayMethod;

            if (_paymentMethod == null)
            {
                var nullRef = new NullReferenceException("PayPalExpressCheckoutPaymentGatewayMethod could not be instantiated");
                MultiLogHelper.Error<PayPalExpressApiController>("PayPalExpressCheckoutPaymentGatewayMethod was null", nullRef, GetExtendedLoggerData());

                throw nullRef;
            }
        }
    }
}