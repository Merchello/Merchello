using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

using Merchello.Core;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.PayPal.Provider;

namespace Merchello.Plugin.Payments.PayPal.Controllers
{
    /// <summary>
    /// The PayPal API controller.
    /// </summary>
    [PluginController("MerchelloPayPal")]
    public class PayPalApiController : UmbracoApiController
    {
        /// <summary>
        /// Merchello context
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// The PayPal payment processor.
        /// </summary>
        private readonly PayPalPaymentProcessor _processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalApiController"/> class.
        /// </summary>
        public PayPalApiController(): this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        public PayPalApiController(IMerchelloContext merchelloContext)
        {
            if (merchelloContext == null) throw new ArgumentNullException("merchelloContext");

            var provider = (PayPalPaymentGatewayProvider)merchelloContext.Gateways.Payment.GetProviderByKey(Constants.GatewayProviderSettingsKey);

            if (provider  == null)
            {
                var ex = new NullReferenceException("The PayPalPaymentGatewayProvider could not be resolved.  The provider must be activiated");
                LogHelper.Error<PayPalApiController>("PayPalPaymentGatewayProvider not activated.", ex);
                throw ex;
            }

            _merchelloContext = merchelloContext;
            _processor = new PayPalPaymentProcessor(provider.ExtendedData.GetProcessorSettings());
        }

        /// <summary>
        /// Authorize payment
        /// </summary>
        /// <param name="invoiceKey"></param>
        /// <param name="paymentKey"></param>
        /// <param name="token"></param>
        /// <param name="payerId"></param>
        /// <returns></returns>
        /// <example>/umbraco/MerchelloPayPal/PayPalApi/AuthorizePayment?InvoiceKey=3daeee31-da2c-41d0-a650-52bafaa16dc1&PaymentKey=562e3108-2c68-4f8e-8818-b5685e3df160&token=EC-0NN997417U7730318&PayerID=UBDNS4GA4TB7Y</example>
        [HttpGet]
        public HttpResponseMessage AuthorizePayment(Guid invoiceKey, Guid paymentKey, string token, string payerId)
        {
            var invoice = _merchelloContext.Services.InvoiceService.GetByKey(invoiceKey);
            var payment = _merchelloContext.Services.PaymentService.GetByKey(paymentKey);
            if (invoice == null || payment == null || String.IsNullOrEmpty(token) || String.IsNullOrEmpty(payerId))
            {
                var ex = new NullReferenceException(string.Format("Invalid argument exception. Arguments: invoiceKey={0}, paymentKey={1}, token={2}, payerId={3}.", invoiceKey, paymentKey, token, payerId));
                LogHelper.Error<PayPalApiController>("Payment is not authorized.", ex);
                throw ex;
            }

            // Authorize
            var authorizeResult = _processor.AuthorizePayment(invoice, payment, token, payerId);
            _merchelloContext.Services.GatewayProviderService.Save(payment);
            if (!authorizeResult.Payment.Success)
            {
                LogHelper.Error<PayPalApiController>("Payment is not authorized.", authorizeResult.Payment.Exception);
                throw authorizeResult.Payment.Exception;
            }

            // Capture
            var paymentGatewayMethod = _merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(payment.PaymentMethodKey.Value);
            var captureResult = paymentGatewayMethod.CapturePayment(invoice, payment, payment.Amount, null);
            if (!captureResult.Payment.Success)
            {
                LogHelper.Error<PayPalApiController>("Payment is not captured.", captureResult.Payment.Exception);
                throw captureResult.Payment.Exception;
            }

            // redirect to Site
            var response = Request.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = new Uri(PayPalPaymentProcessor.GetWebsiteUrl() + _processor.Settings.ConfirmationReturnUrl.Replace("%INVOICE%", invoice.Key.ToString().EncryptWithMachineKey()));
            return response;
        }

        /*
        // for test
        [HttpGet]
        public string RefundPayment(Guid invoiceKey, Guid paymentKey)
        {
            var invoice = _merchelloContext.Services.InvoiceService.GetByKey(invoiceKey);
            var payment = _merchelloContext.Services.PaymentService.GetByKey(paymentKey);
            if (invoice == null || payment == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            var result = _processor.RefundPayment(invoice, payment, 0);
            return "";
        }
        */
    }
}
