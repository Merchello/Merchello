namespace Merchello.Example.Controllers
{
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
    using Merchello.Core.Checkout;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Providers.Payment.Braintree;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// A Braintree standard transaction
    /// </summary>
    [PluginController("Example")]
    [GatewayMethodUi("BrainTree.StandardTransaction")]
    public class BraintreeStandardTransactionController : BraintreeTransactionControllerBase
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
            return this.PartialView(this.BraintreePartial("BraintreeStandardTransaction"), model);
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

            var form = UmbracoContext.HttpContext.Request.Form;
            var paymentMethodNonce = form.Get("payment_method_nonce");

            //// ----------------------------------------------------------------------------

            return this.ProcessPayment(checkoutManager, paymentMethod, paymentMethodNonce);
        }
    }
}