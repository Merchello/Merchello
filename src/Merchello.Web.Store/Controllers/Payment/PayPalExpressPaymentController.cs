namespace Merchello.Web.Store.Controllers.Payment
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Providers.Payment.PayPal.Models;
    using Merchello.Web.Controllers;
    using Merchello.Web.Store.Models;

    using Umbraco.Core;
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

                var attempt = CheckoutManager.Payment.AuthorizePayment(paymentMethod.Key, args);

                var resultModel = CheckoutPaymentModelFactory.Create(CurrentCustomer, paymentMethod, attempt);
                
                if (attempt.Payment.Success)
                {
                    CustomerContext.SetValue("invoiceKey", attempt.Invoice.Key.ToString());
                    return Redirect(attempt.RedirectUrl);
                }

                return HandlePaymentSuccess(resultModel);
            }
            catch (Exception ex)
            {
                return HandlePaymentException(model, ex);
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
    }
}