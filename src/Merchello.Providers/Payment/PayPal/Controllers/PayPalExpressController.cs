namespace Merchello.Providers.Payment.PayPal.Controllers
{
    using System;
    using System.Net;
    using System.Web.Mvc;

    using global::PayPal.PayPalAPIInterfaceService.Model;

    using Merchello.Core;
    using Merchello.Core.Events;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Providers.Models;
    using Merchello.Providers.Payment.PayPal.Models;
    using Merchello.Providers.Payment.PayPal.Provider;
    using Merchello.Providers.Payment.PayPal.Services;
    using Merchello.Web.Models.Ui.Async;
    using Merchello.Web.Mvc;

    using Umbraco.Core.Events;
    using Umbraco.Web.Mvc;

    using Constants = Merchello.Providers.Constants;

    /// <summary>
    /// A surface controller for used for accepting PayPal Express Payments.
    /// </summary>
    [PluginController("Merchello")]
    public class PayPalExpressController : PayPalSurfaceControllerBase
    {
        /// <summary>
        /// The <see cref="IPayPalApiPaymentService"/>.
        /// </summary>
        private IPayPalApiService _paypalApiService;

        /// <summary>
        /// The URL for a Success return.
        /// </summary>
        private string _successUrl;

        /// <summary>
        /// The URL for a Cancel return.
        /// </summary>
        private string _cancelUrl;

        /// <summary>
        /// A value indicating whether or not to delete the invoice on cancel.
        /// </summary>
        /// <remarks>
        /// If false the authorize payment is voided.
        /// </remarks>
        private bool _deleteInvoiceOnCancel;

        /// <summary>
        /// The <see cref="PayPalExpressCheckoutPaymentGatewayMethod"/>.
        /// </summary>
        private PayPalExpressCheckoutPaymentGatewayMethod _paymentMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalExpressController"/> class.
        /// </summary>
        public PayPalExpressController()
        {
            this.Initialize();
        }

        /// <summary>
        /// Occurs before redirecting for a successful response.
        /// </summary>
        public static event TypedEventHandler<PayPalExpressController, ObjectEventArgs<PaymentRedirectingUrl>> RedirectingForSuccess;

        /// <summary>
        /// Occurs before redirecting for a cancel response.
        /// </summary>
        public static event TypedEventHandler<PayPalExpressController, ObjectEventArgs<PaymentRedirectingUrl>> RedirectingForCancel;

        /// <summary>
        /// Occurs after the final redirection and before redirecting to the success URL
        /// </summary>
        /// <remarks>
        /// Can be used to send OrderConfirmation notification
        /// </remarks>
        public static event TypedEventHandler<PayPalExpressController, PaymentAttemptEventArgs<IPaymentResult>> Processed;
             
        /// <summary>
        /// Handles the a successful payment response from the PayPal Express checkout
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
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public override ActionResult Success(Guid invoiceKey, Guid paymentKey, string token, string payerId)
        {
            var redirecting = new PaymentRedirectingUrl("Success") { RedirectingToUrl = _successUrl };

            var logData = GetExtendedLoggerData();

            try
            {
                var invoice = GetInvoice(invoiceKey);
                var payment = GetPayment(paymentKey);

                // We can now capture the payment
                // This will actually make a few more API calls back to PayPal to get required transaction
                // data so that we can refund the payment later through the back office if needed.
                var attempt = invoice.CapturePayment(payment, _paymentMethod, invoice.Total);

                // Raise the event to process the email
                Processed.RaiseEvent(new PaymentAttemptEventArgs<IPaymentResult>(attempt), this);

                // If this is an AJAX request return the JSON
                if (payment.ExtendedData.GetPayPalRequestIsAjaxRequest())
                {
                    var resp = new PaymentResultAsyncResponse
                    {
                        Success = attempt.Payment.Success,
                        InvoiceKey = attempt.Invoice.Key,
                        PaymentKey = attempt.Payment.Result.Key,
                        PaymentMethodName = "PayPal Express Checkout"
                    };

                    if (attempt.Payment.Exception != null)
                        resp.Messages.Add(attempt.Payment.Exception.Message);

                    return Json(resp);
                }

                if (attempt.Payment.Success)
                {
                    // we need to empty the basket here
                    Basket.Empty();
                    redirecting.RedirectingToUrl = $"{redirecting.RedirectingToUrl}?invoiceKey={invoiceKey}";
                    // raise the event so the redirect URL can be manipulated
                    RedirectingForSuccess.RaiseEvent(new ObjectEventArgs<PaymentRedirectingUrl>(redirecting), this);

                    return Redirect(redirecting.RedirectingToUrl);
                }

                var retrying = new PaymentRedirectingUrl("Cancel") { RedirectingToUrl = _cancelUrl };
                var qs = string.Format("?invoicekey={0}&paymentkey={1}", invoiceKey, paymentKey);
                if (!retrying.RedirectingToUrl.IsNullOrWhiteSpace()) return Redirect(retrying.RedirectingToUrl + qs);

                var invalidOp = new InvalidOperationException("Retry url was not specified");

                MultiLogHelper.Error<PayPalExpressController>("Could not redirect to retry", invalidOp);
                throw invalidOp;
            }
            catch (Exception ex)
            {
                var extra = new { InvoiceKey = invoiceKey, PaymentKey = paymentKey, Token = token, PayerId = payerId };

                logData.SetValue<object>("extra", extra);

                MultiLogHelper.Error<PayPalExpressController>(
                    "Failed to Capture SUCCESSFUL PayPal Express checkout response.",
                    ex,
                    logData);

                throw;
            }
        }

        /// <summary>
        /// Handles a cancellation payment response from the PayPal Express checkout.
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
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public override ActionResult Cancel(Guid invoiceKey, Guid paymentKey, string token, string payerId = null)
        {
            var redirecting = new PaymentRedirectingUrl("Cancel") { RedirectingToUrl = _cancelUrl };

            try
            {
                var invoice = GetInvoice(invoiceKey);
                var payment = GetPayment(paymentKey);

                if (_deleteInvoiceOnCancel)
                {
                    InvoiceService.Delete(invoice);
                }
                else
                {
                    payment.VoidPayment(invoice, _paymentMethod.PaymentMethod.Key);
                }
               
                // raise the event so the redirect URL can be manipulated
                RedirectingForCancel.RaiseEvent(new ObjectEventArgs<PaymentRedirectingUrl>(redirecting), this);
                return Redirect(redirecting.RedirectingToUrl);
            }
            catch (Exception ex)
            {
                var logData = GetExtendedLoggerData();

                var extra = new { InvoiceKey = invoiceKey, PaymentKey = paymentKey, Token = token, PayerId = payerId };

                logData.SetValue<object>("extra", extra);

                MultiLogHelper.Error<PayPalExpressController>(
                    "Failed to Cancel PayPal Express checkout response.",
                    ex,
                    logData);

                throw;
            }
        }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        private void Initialize()
        {
            
            var provider = GatewayContext.Payment.GetProviderByKey(Constants.PayPal.GatewayProviderSettingsKey) as PayPalPaymentGatewayProvider;
            if (provider == null)
            {
                var nullRef =
                    new NullReferenceException(
                        "PayPalPaymentGatewayProvider is not activated or has not been resolved.");
                MultiLogHelper.Error<PayPalExpressController>(
                    "Failed to find active PayPalPaymentGatewayProvider.",
                    nullRef,
                    GetExtendedLoggerData());

                throw nullRef;
            }

            // instantiate the service
            _paypalApiService = new PayPalApiService(provider.ExtendedData.GetPayPalProviderSettings());

            var settings = provider.ExtendedData.GetPayPalProviderSettings();
            _successUrl = settings.SuccessUrl;
            _cancelUrl = settings.CancelUrl;
            _deleteInvoiceOnCancel = settings.DeleteInvoiceOnCancel;

            _paymentMethod = provider.GetPaymentGatewayMethodByPaymentCode(Constants.PayPal.PaymentCodes.ExpressCheckout) as PayPalExpressCheckoutPaymentGatewayMethod;

            if (_paymentMethod == null)
            {
                var nullRef = new NullReferenceException("PayPalExpressCheckoutPaymentGatewayMethod could not be instantiated");
                MultiLogHelper.Error<PayPalExpressController>("PayPalExpressCheckoutPaymentGatewayMethod was null", nullRef, GetExtendedLoggerData());

                throw nullRef;
            }
        }
    }
}