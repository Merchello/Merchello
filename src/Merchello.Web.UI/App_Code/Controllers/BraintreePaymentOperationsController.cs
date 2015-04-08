namespace Merchello.Introduction.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Bazaar.Controllers;
    using Merchello.Bazaar.Models;
    using Merchello.Core;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;
    using Merchello.Introduction.Models;
    using Merchello.Plugin.Payments.Braintree;
    using Merchello.Plugin.Payments.Braintree.Provider;
    using Merchello.Plugin.Payments.Braintree.Services;
    using Merchello.Web;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The braintree payment operations controller.
    /// </summary>
    [PluginController("MerchelloIntroduction")]
    public class BraintreePaymentOperationsController : SurfaceControllerBase
    {
        /// <summary>
        /// The <see cref="BraintreePaymentGatewayProvider"/>.
        /// </summary>
        private readonly BraintreePaymentGatewayProvider _provider;

        /// <summary>
        /// The <see cref="IBraintreeApiService"/>
        /// </summary>
        private readonly IBraintreeApiService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentOperationsController"/> class.
        /// </summary>
        public BraintreePaymentOperationsController()
        {
            //// D143E0F6-98BB-4E0A-8B8C-CE9AD91B0969 is the Guid from the BraintreeProvider Activation Attribute
            //// [GatewayProviderActivation("D143E0F6-98BB-4E0A-8B8C-CE9AD91B0969", "BrainTree Payment Provider", "BrainTree Payment Provider")]
            _provider = (BraintreePaymentGatewayProvider)MerchelloContext.Current.Gateways.Payment.GetProviderByKey(new Guid("D143E0F6-98BB-4E0A-8B8C-CE9AD91B0969"));

            // GetBraintreeProviderSettings() is an extension method with the provider
            _service = new BraintreeApiService(_provider.ExtendedData.GetBrainTreeProviderSettings());
        }

        [ChildActionOnly]
        public ActionResult RenderBraintreeSetupJs()
        {
            var token = CurrentCustomer.IsAnonymous ?
            _service.Customer.GenerateClientRequestToken() :
            _service.Customer.GenerateClientRequestToken((ICustomer)CurrentCustomer);

            return this.PartialView("BraintreeSetupJs", new BraintreeTokenModel() { Token = token });
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