namespace Merchello.Providers.Payment.Braintree.Provider
{
    using System;

    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Providers.Payment.Braintree.Services;

    using Umbraco.Core.Logging;

    /// <summary>
    /// Payment method for Braintree PayPal OneTime checkouts.
    /// </summary>
    [GatewayMethodUi("BrainTree.PayPal.OneTime")]
    [GatewayMethodEditor("BrainTree PayPal OneTime Payment Method Editor", "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.paymentmethod.addedit.html")]
    [PaymentGatewayMethod("Braintree PayPal Gateway Method Editors",
        "",  // cannot authorize a payment
        "",  // cannot authorize capture since we don't have their user/pass
        "~/App_Plugins/MerchelloProviders/views/dialogs/braintree.standard.voidpayment.html",
        "~/App_Plugins/MerchelloProviders/views/dialogs/braintree.standard.refundpayment.html",
        "")]
    public class PayPalOneTimeTransactionPaymentGatewayMethod : BraintreeStandardPaymentGatewayBase, IPayPalOneTimeTransactionPaymentGatewayMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalOneTimeTransactionPaymentGatewayMethod"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        /// <param name="braintreeApiService">
        /// The braintree api service.
        /// </param>
        public PayPalOneTimeTransactionPaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, IBraintreeApiService braintreeApiService)
            : base(gatewayProviderService, paymentMethod, braintreeApiService)
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the payment line authorize description.
        /// </summary>
        protected override string PaymentLineAuthorizeDescription { get; set; }

        /// <summary>
        /// Gets or sets the payment line authorize capture description.
        /// </summary>
        protected override string PaymentLineAuthorizeCaptureDescription { get; set; }

        /// <summary>
        /// Overrides the AuthorizePayment.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Throws an exception if this method is called.
        /// </exception>
        public override IPaymentResult AuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            var invalidOp =
                new InvalidOperationException(
                    "Braintree PayPal authorize transaction is not supported.  Use AuthorizeCapture Payment");

            LogHelper.Error<PayPalVaultTransactionPaymentGatewayMethod>("Authorize method not supported.", invalidOp);

            throw invalidOp;
        }

        /// <summary>
        /// Initializes the gateway method.
        /// </summary>
        private void Initialize()
        {
            this.PaymentLineAuthorizeCaptureDescription =
                "Braintree PayPal one time transaction - authorized and captured";
        }
    }
}