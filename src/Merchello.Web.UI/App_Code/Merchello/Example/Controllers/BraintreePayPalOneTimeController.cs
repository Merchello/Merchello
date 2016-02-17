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
            return this.PartialView(this.BraintreePartial("BraintreePayPalOneTime"), model);
        }

        protected override IPaymentResult PerformProcessPayment(ICheckoutManagerBase checkoutManager, IPaymentMethod paymentMethod)
        {
            // ----------------------------------------------------------------------------
            // WE NEED TO GET THE PAYMENT METHOD "NONCE" FOR BRAINTREE

            var form = UmbracoContext.HttpContext.Request.Form;
            var paymentMethodNonce = form.Get("payment_method_nonce");

            // ----------------------------------------------------------------------------

            return this.ProcessPayment(checkoutManager, paymentMethod, paymentMethodNonce);
        }

        /// <summary>
        /// The process payment.
        /// </summary>
        /// <param name="checkoutManager">
        /// Merchello's <see cref="ICheckoutManagerBase"/>.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        /// <remarks>
        /// AuthorizeCapturePayment will save the invoice with an Invoice Number.
        /// </remarks>
        private IPaymentResult ProcessPayment(ICheckoutManagerBase checkoutManager, IPaymentMethod paymentMethod, string paymentMethodNonce)
        {
            // You need a ProcessorArgumentCollection for this transaction to store the payment method nonce
            // The braintree package includes an extension method off of the ProcessorArgumentCollection - SetPaymentMethodNonce([nonce]);
            var args = new ProcessorArgumentCollection();
            args.SetPaymentMethodNonce(paymentMethodNonce);

            // We will want this to be an AuthorizeCapture(paymentMethod.Key, args);
            return checkoutManager.Payment.AuthorizeCapturePayment(paymentMethod.Key, args);
        }
    }

}