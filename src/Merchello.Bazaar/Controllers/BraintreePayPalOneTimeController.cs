namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
    using Merchello.Core.Checkout;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// A Braintree standard transaction
    /// </summary>
    [PluginController("Bazaar")]
    [GatewayMethodUi("Braintree.PayPal.OneTime")]
    public class BraintreePayPalOneTimeController : BraintreeTransactionControllerBase
    {
        /// <summary>
        /// Renders the payment form.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public override ActionResult RenderForm(CheckoutConfirmationForm model)
        {
            return this.PartialView(PathHelper.GetThemePartialViewPath(BazaarContentHelper.GetStoreTheme(), "BraintreePayPalOneTime"), model);
        }

        /// <summary>
        /// Collects the Braintree Token (nonce) and processes the payment.
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
            //// ----------------------------------------------------------------------------
            //// WE NEED TO GET THE PAYMENT METHOD "NONCE" FOR BRAINTREE

            var form = this.UmbracoContext.HttpContext.Request.Form;
            var paymentMethodNonce = form.Get("paypal_payment_method_nonce");

            //// ----------------------------------------------------------------------------

            return this.ProcessPayment(checkoutManager, paymentMethod, paymentMethodNonce);
        }
    }
}