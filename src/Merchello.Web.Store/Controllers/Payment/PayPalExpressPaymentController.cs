﻿namespace Merchello.Web.Store.Controllers.Payment
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Logging;
    using Merchello.Providers.Models;
    using Merchello.Providers.Payment.PayPal.Models;
    using Merchello.Web.Controllers;
    using Merchello.Web.Store.Models;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// A controller for rendering and processing PayPal Express Checkout.
    /// </summary>
    [PluginController("Merchello")]
    [GatewayMethodUi("PayPal.ExpressCheckout")]
    public class PayPalExpressPaymentController : CheckoutPaymentControllerBase<StorePaymentModel>
    {
        /// <summary>
        /// Processes the PayPal Express Payment.
        /// </summary>
        /// <param name="model">
        /// The <see cref="StorePaymentModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Process(StorePaymentModel model)
        {

            try
            {
                var paymentMethod = CheckoutManager.Payment.GetPaymentMethod();
                if (paymentMethod == null)
                {
                    var ex = new NullReferenceException("PaymentMethod was null");
                    return HandlePaymentException(model, ex);
                }

                var args = new ProcessorArgumentCollection();
                if (Request.IsAjaxRequest())
                {
                    args.SetPayPalExpressAjaxRequest(true);
                }

                // Don't empty the basket here.
                CheckoutManager.Context.Settings.EmptyBasketOnPaymentSuccess = false;

                var attempt = CheckoutManager.Payment.AuthorizePayment(paymentMethod.Key, args);

                var resultModel = CheckoutPaymentModelFactory.Create(CurrentCustomer, paymentMethod, attempt);

                if (attempt.Payment.Success)
                {
                    CustomerContext.SetValue("invoiceKey", attempt.Invoice.Key.ToString());
                    return Redirect(attempt.RedirectUrl);
                }

                if (!resultModel.ViewData.Success)
                {
                    var invoiceKey = attempt.Invoice.Key;
                    var paymentKey = attempt.Payment.Result != null ? attempt.Payment.Result.Key : Guid.Empty;
                    EnsureDeleteInvoiceOnCancel(invoiceKey, paymentKey);
                }

                return HandlePaymentSuccess(resultModel);
            }
            catch (Exception ex)
            {
                return HandlePaymentException(model, ex);
            }
        }

        /// <summary>
        /// The retry.
        /// </summary>
        /// <param name="invoiceKey">
        /// The key for the invoice created.
        /// </param>
        /// <param name="paymentKey">
        /// The payment key.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public virtual ActionResult Retry(Guid invoiceKey, Guid paymentKey)
        {
            try
            {
                var settings = EnsureDeleteInvoiceOnCancel(invoiceKey, paymentKey);


                return Redirect(!settings.RetryUrl.IsNullOrWhiteSpace() ? 
                    settings.RetryUrl :
                    "/");
            }
            catch (Exception ex)
            {
                var logData = GetExtendedLoggerData();
                MultiLogHelper.Error<PayPalExpressPaymentController>("PayPal Express checkout retry failed.", ex, logData);
                throw;
            }
        }

        /// <summary>
        /// Renders the PayPal Express payment form.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        [GatewayMethodUi("PayPal.ExpressCheckout")]
        public override ActionResult PaymentForm(string view = "")
        {
            var paymentMethod = this.CheckoutManager.Payment.GetPaymentMethod();
            if (paymentMethod == null) return this.InvalidCheckoutStagePartial();

            var model = this.CheckoutPaymentModelFactory.Create(CurrentCustomer, paymentMethod);

            return view.IsNullOrWhiteSpace() ? this.PartialView(model) : this.PartialView(view, model);
        }

        /// <summary>
        /// Special case where there is a failure at PayPal end and PayPal still returns HTTP 200 but ACK != Success
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="ex">
        /// The exception.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected override ActionResult HandlePaymentException(StorePaymentModel model, Exception ex)
        {
            var invoiceKey = model.ViewData.InvoiceKey;
            var paymentKey = model.ViewData.PaymentKey;
            EnsureDeleteInvoiceOnCancel(invoiceKey, paymentKey);
            return base.HandlePaymentException(model, ex);
        }

        /// <summary>
        /// Deletes the invoice on cancel.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="paymentKey">
        /// The payment key.
        /// </param>
        /// <returns>
        /// The <see cref="PayPalProviderSettings"/>.
        /// </returns>
        private PayPalProviderSettings EnsureDeleteInvoiceOnCancel(Guid invoiceKey, Guid paymentKey)
        {
            var provider = GatewayContext.Payment.GetProviderByKey(Merchello.Providers.Constants.PayPal.GatewayProviderSettingsKey);
            var settings = provider.ExtendedData.GetPayPalProviderSettings();

            if (settings.DeleteInvoiceOnCancel)
            {
                // validate that this invoice should be deleted
                var invoice = MerchelloServices.InvoiceService.GetByKey(invoiceKey);

                var payments = invoice.Payments().ToArray();

                // we don't want to delete if there is more than one payment
                if (payments.Count() <= 1)
                {
                    // Assert the payment key matches - this is to ensure that the 
                    // payment matches the invoice
                    var ensure = payments.All(x => x.Key == paymentKey) || !payments.Any();
                    if (ensure && invoice.InvoiceStatus.Key == Core.Constants.InvoiceStatus.Unpaid)
                    {
                        MultiLogHelper.Info<PayPalExpressPaymentController>(string.Format("Deleted invoice number {0} to prevent duplicate. PayPal ACK response not success", invoice.PrefixedInvoiceNumber()));
                        MerchelloServices.InvoiceService.Delete(invoice);
                    }
                }
            }

            return settings;
        }
    }
}