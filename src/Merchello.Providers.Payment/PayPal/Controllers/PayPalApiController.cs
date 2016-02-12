namespace Merchello.Providers.Payment.PayPal.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Providers.Payment.PayPal.Provider;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.WebApi;

    using Constants = Payment.Constants;

    /// <summary>
    /// The PayPal API controller.
    /// </summary>
    [PluginController("MerchelloPayments")]
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

	        var providerKey = new Guid(Constants.PayPal.PayPalPaymentGatewayProviderKey);
            var provider = (PayPalPaymentGatewayProvider)merchelloContext.Gateways.Payment.GetProviderByKey(providerKey);

            if (provider  == null)
            {
                var ex = new NullReferenceException("The PayPalPaymentGatewayProvider could not be resolved.  The provider must be activiated");
                LogHelper.Error<PayPalApiController>("PayPalPaymentGatewayProvider not activated.", ex);
                throw ex;
            }

            this._merchelloContext = merchelloContext;
            this._processor = new PayPalPaymentProcessor(provider.ExtendedData.GetProcessorSettings());
        }

		
        /// <summary>
        /// Authorize payment
        /// </summary>
        /// <param name="invoiceKey"></param>
        /// <param name="paymentKey"></param>
        /// <param name="token"></param>
        /// <param name="payerId"></param>
        /// <returns></returns>
        /// <example>/umbraco/MerchelloPayPal/PayPalApi/SuccessPayment?InvoiceKey=3daeee31-da2c-41d0-a650-52bafaa16dc1&PaymentKey=562e3108-2c68-4f8e-8818-b5685e3df160&token=EC-0NN997417U7730318&PayerID=UBDNS4GA4TB7Y</example>
        [HttpGet]
        public HttpResponseMessage SuccessPayment(Guid invoiceKey, Guid paymentKey, string token, string payerId)
        {
            var invoice = this._merchelloContext.Services.InvoiceService.GetByKey(invoiceKey);
            var payment = this._merchelloContext.Services.PaymentService.GetByKey(paymentKey);
            if (invoice == null || payment == null || String.IsNullOrEmpty(token) || String.IsNullOrEmpty(payerId))
            {
                var ex = new NullReferenceException(string.Format("Invalid argument exception. Arguments: invoiceKey={0}, paymentKey={1}, token={2}, payerId={3}.", invoiceKey, paymentKey, token, payerId));
                LogHelper.Error<PayPalApiController>("Payment is not authorized.", ex);
                throw ex;
            }

	        var providerKeyGuid = new Guid(Constants.PayPal.PayPalPaymentGatewayProviderKey);
			var paymentGatewayMethod = this._merchelloContext.Gateways.Payment
				.GetPaymentGatewayMethods()
				.First(item => item.PaymentMethod.ProviderKey == providerKeyGuid);
	        //var paymentGatewayMethod = _merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(providerKeyGuid);

            // Authorize
            var authorizeResult = this._processor.AuthorizePayment(invoice, payment, token, payerId);
	        /*
			var authorizePaymentProcArgs = new ProcessorArgumentCollection();

	        authorizePaymentProcArgs[Constants.ProcessorArgumentsKeys.internalTokenKey] = token;
			authorizePaymentProcArgs[Constants.ProcessorArgumentsKeys.internalPayerIDKey] = payerId;
			authorizePaymentProcArgs[Constants.ProcessorArgumentsKeys.internalPaymentKeyKey] = payment.Key.ToString();

	        var authorizeResult = paymentGatewayMethod.AuthorizeCapturePayment(invoice, payment.Amount, authorizePaymentProcArgs);
            */
            this._merchelloContext.Services.GatewayProviderService.Save(payment);
            if (!authorizeResult.Payment.Success)
            {
                LogHelper.Error<PayPalApiController>("Payment is not authorized.", authorizeResult.Payment.Exception);
				this._merchelloContext.Services.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, "PayPal: request capture authorization error: " + authorizeResult.Payment.Exception.Message, 0);
                return this.ShowError(authorizeResult.Payment.Exception.Message);
            }
			this._merchelloContext.Services.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "PayPal: capture authorized", 0);

			// The basket can be empty
            var customerContext = new Merchello.Web.CustomerContext(this.UmbracoContext);
            var currentCustomer = customerContext.CurrentCustomer;
	        if (currentCustomer != null) {
				var basket = Merchello.Web.Workflow.Basket.GetBasket(currentCustomer);
				basket.Empty();
	        }

            // Capture
            var captureResult = paymentGatewayMethod.CapturePayment(invoice, payment, payment.Amount, null);
            if (!captureResult.Payment.Success)
            {
                LogHelper.Error<PayPalApiController>("Payment is not captured.", captureResult.Payment.Exception);
                return this.ShowError(captureResult.Payment.Exception.Message);
            }

            // redirect to Site
			var returnUrl = payment.ExtendedData.GetValue(Constants.PayPal.ExtendedDataKeys.ReturnUrl);
            var response = this.Request.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = new Uri(returnUrl.Replace("%INVOICE%", invoice.Key.ToString().EncryptWithMachineKey()));
            return response;
        }
		
        /// <summary>
        /// Abort payment
        /// </summary>
        /// <param name="invoiceKey"></param>
        /// <param name="paymentKey"></param>
        /// <param name="token"></param>
        /// <param name="payerId"></param>
        /// <returns></returns>
        /// <example>/umbraco/MerchelloPayPal/PayPalApi/AbortPayment?InvoiceKey=3daeee31-da2c-41d0-a650-52bafaa16dc1&PaymentKey=562e3108-2c68-4f8e-8818-b5685e3df160&token=EC-0NN997417U7730318&PayerID=UBDNS4GA4TB7Y</example>
        [HttpGet]
        public HttpResponseMessage AbortPayment(Guid invoiceKey, Guid paymentKey, string token, string payerId = null)
        {
            
			var invoiceService = this._merchelloContext.Services.InvoiceService;
	        var paymentService = this._merchelloContext.Services.PaymentService;

			var invoice = invoiceService.GetByKey(invoiceKey);
            var payment = paymentService.GetByKey(paymentKey);
            if (invoice == null || payment == null || String.IsNullOrEmpty(token))
            {
                var ex = new NullReferenceException(string.Format("Invalid argument exception. Arguments: invoiceKey={0}, paymentKey={1}, token={2}, payerId={3}.", invoiceKey, paymentKey, token, payerId));
                LogHelper.Error<PayPalApiController>("Payment is not authorized.", ex);
                return this.ShowError(ex.Message);
            }

			// Delete invoice
			invoiceService.Delete(invoice);

			// Return to CancelUrl
	        var cancelUrl = payment.ExtendedData.GetValue(Constants.PayPal.ExtendedDataKeys.CancelUrl);
            var response = this.Request.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = new Uri(cancelUrl);
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

        // TODO: add link to Error page
        private HttpResponseMessage ShowError(string message)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent("Error: " + message, Encoding.UTF8, "text/plain");
            return resp;
        }
    }
}