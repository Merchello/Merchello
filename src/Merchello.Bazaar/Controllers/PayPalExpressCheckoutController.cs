namespace Merchello.Bazaar.Controllers
{
    using System;
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
    using Merchello.Core;
    using Merchello.Core.Checkout;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;

    using Umbraco.Web.Mvc;

    using PathHelper = Merchello.Bazaar.PathHelper;

    /// <summary>
    /// A controller responsible for rending the .
    /// </summary>
    [PluginController("Bazaar")]
    [GatewayMethodUi("PayPal.ExpressCheckout")]
    public class PayPalExpressCheckoutController : BazaarPaymentMethodFormControllerBase
    {
        /// <summary>
        /// Renders the PayPal Express button.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public override ActionResult RenderForm(CheckoutConfirmationForm model)
        {
            return this.PartialView(PathHelper.GetThemePartialViewPath(BazaarContentHelper.GetStoreTheme(), "PayPalExpressCheckout"), model);
        }

        /// <summary>
        /// Finalizes the sale and performs the redirection.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="attempt">
        /// The attempt.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <remarks>
        /// This overrides the default behavior to redirect to PayPal
        /// </remarks>
        protected override ActionResult FinalizeSale(CheckoutConfirmationForm model, IPaymentResult attempt)
        {
            if (attempt.Payment.Success)
            {
                CustomerContext.SetValue("invoiceKey", attempt.Invoice.Key.ToString());

                // Trigger the order confirmation notification
                var billingAddress = attempt.Invoice.GetBillingAddress();
                string contactEmail;
                if (string.IsNullOrEmpty(billingAddress.Email) && !CurrentCustomer.IsAnonymous)
                {
                    contactEmail = ((ICustomer)CurrentCustomer).Email;
                }
                else
                {
                    contactEmail = billingAddress.Email;
                }

                if (!string.IsNullOrEmpty(contactEmail))
                {
                    Notification.Trigger("OrderConfirmation", attempt, new[] { contactEmail });
                }

                return Redirect(attempt.RedirectUrl);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs the payment.
        /// </summary>
        /// <param name="checkoutManager">
        /// The checkout manager.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        protected override IPaymentResult PerformProcessPayment(ICheckoutManagerBase checkoutManager, IPaymentMethod paymentMethod)
        {

            return checkoutManager.Payment.AuthorizePayment(paymentMethod.Key);
        }
    }
}