namespace Merchello.Plugin.Payments.Braintree.Bazaar.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;
    using Merchello.Web;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// A controller to Authorize/Capture Braintree payments.
    /// </summary>
    [GatewayMethodUi("BrainTree.StandardTransaction")]
    [PluginController("Bazaar")]
    public class BraintreeStandardTransactionController : BraintreeTransactionControllerBase
    {        
        /// <summary>
        /// Responsible for rendering the payment form for capturing standard payments via Braintree.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/> Partial View.
        /// </returns>
        [ChildActionOnly]
        public override ActionResult RenderForm(CheckoutConfirmationForm model)
        {
            return this.PartialView(BraintreePartial("BraintreeStandardTransaction"), model);
        }

        /// <summary>
        /// The confirm sale.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public ActionResult ConfirmSale(CheckoutConfirmationForm model)
        {
            if (!ModelState.IsValid) return this.CurrentUmbracoPage();

            var preparation = Basket.SalePreparation();
            preparation.RaiseCustomerEvents = false;

            ////
            // SHIPPING INFORMATION NEEDS TO BE SAVED AGAIN
            ////
            preparation.ClearShipmentRateQuotes();
            var shippingAddress = Basket.SalePreparation().GetShipToAddress();

            // Get the shipment again
            var shipment = Basket.PackageBasket(shippingAddress).FirstOrDefault();

            // get the quote using the "approved shipping method"
            var quote = shipment.ShipmentRateQuoteByShipMethod(model.ShipMethodKey);

            // save the quote
            Basket.SalePreparation().SaveShipmentRateQuote(quote);

            var paymentMethod = GatewayContext.Payment.GetPaymentGatewayMethodByKey(model.PaymentMethodKey).PaymentMethod;
            preparation.SavePaymentMethod(paymentMethod);

            // ----------------------------------------------------------------------------
            // WE NEED TO GET THE PAYMENT METHOD "NONCE" FOR BRAINTREE

            var form = UmbracoContext.HttpContext.Request.Form;
            var paymentMethodNonce = form.Get("payment_method_nonce");

            // ----------------------------------------------------------------------------

            var attempt = this.ProcessPayment(preparation, paymentMethod, paymentMethodNonce);

            // Trigger the order confirmation notification
            var billingAddress = attempt.Invoice.GetBillingAddress();
            if (!string.IsNullOrEmpty(billingAddress.Email))
            {
                Notification.Trigger("OrderConfirmation", attempt.Payment, new[] { billingAddress.Email });
            }

            // store the invoice key in the CustomerContext for use on the receipt page.
            CustomerContext.SetValue("invoiceKey", attempt.Invoice.Key.ToString());

            return RedirectToUmbracoPage(model.ReceiptPageId);
        }

        // AuthorizeCapturePayment will save the invoice with an Invoice Number.
        private IPaymentResult ProcessPayment(SalePreparationBase preparation, IPaymentMethod paymentMethod, string paymentMethodNonce)
        {
            // You need a ProcessorArgumentCollection for this transaction to store the payment method nonce
            // The braintree package includes an extension method off of the ProcessorArgumentCollection - SetPaymentMethodNonce([nonce]);
            var args = new ProcessorArgumentCollection();
            args.SetPaymentMethodNonce(paymentMethodNonce);

            // We will want this to be an AuthorizeCapture(paymentMethod.Key, args);
            return preparation.AuthorizeCapturePayment(paymentMethod.Key, args);
        }
    }
}