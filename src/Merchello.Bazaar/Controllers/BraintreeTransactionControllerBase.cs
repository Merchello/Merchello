namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
    using Merchello.Core;
    using Merchello.Core.Checkout;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Providers.Models;
    using Merchello.Providers.Payment.Braintree;
    using Merchello.Providers.Payment.Braintree.Provider;
    using Merchello.Providers.Payment.Braintree.Services;
    using Merchello.Providers.Payment.Models;

    using PathHelper = Merchello.Bazaar.PathHelper;

    /// <summary>
    /// Example Braintree transaction base controller
    /// </summary>
    public abstract class BraintreeTransactionControllerBase : BazaarPaymentMethodFormControllerBase
    {
        /// <summary>
        /// The <see cref="IBraintreeApiService"/>
        /// </summary>
        private readonly IBraintreeApiService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeTransactionControllerBase"/> class.
        /// </summary>
        protected BraintreeTransactionControllerBase()
        {
            //// D143E0F6-98BB-4E0A-8B8C-CE9AD91B0969 is the Guid from the BraintreeProvider Activation Attribute
            //// [GatewayProviderActivation("D143E0F6-98BB-4E0A-8B8C-CE9AD91B0969", "BrainTree Payment Provider", "BrainTree Payment Provider")]
            var provider = (BraintreePaymentGatewayProvider)MerchelloContext.Current.Gateways.Payment.GetProviderByKey(Providers.Constants.Braintree.GatewayProviderSettingsKey);

            // GetBraintreeProviderSettings() is an extension method with the provider
            this._service = new BraintreeApiService(provider.ExtendedData.GetBrainTreeProviderSettings());
        }

        /// <summary>
        /// Renders Braintree setup js.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public ActionResult RenderBraintreeSetupJs()
        {
            return this.PartialView(PathHelper.GetThemePartialViewPath(BazaarContentHelper.GetStoreTheme(), "BraintreeSetupJs"), this.GetToken());
        }

        /// <summary>
        /// Renders Braintree PayPal setup js.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public ActionResult RenderPayPalSetupJs()
        {
            return this.PartialView(PathHelper.GetThemePartialViewPath(BazaarContentHelper.GetStoreTheme(), "BraintreePayPalSetupJs"), this.GetToken());
        }

        /// <summary>
        /// Gets the Braintree Server token.
        /// </summary>
        /// <returns>
        /// The <see cref="BraintreeToken"/>.
        /// </returns>
        public BraintreeToken GetToken()
        {
            var token = this.CurrentCustomer.IsAnonymous ?
            this._service.Customer.GenerateClientRequestToken() :
            this._service.Customer.GenerateClientRequestToken((ICustomer)this.CurrentCustomer);

            return new BraintreeToken { Token = token };
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
        protected virtual IPaymentResult ProcessPayment(ICheckoutManagerBase checkoutManager, IPaymentMethod paymentMethod, string paymentMethodNonce)
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