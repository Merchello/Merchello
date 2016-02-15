namespace Merchello.Providers.Payment.Braintree.Provider
{
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Providers.Payment.Braintree.Models;
    using Merchello.Providers.Payment.Braintree.Services;

    /// <summary>
    /// Payment method for Braintree PayPal OneTime checkouts.
    /// </summary>
    [GatewayMethodUi("Braintree.PayPal.OneTime")]
    [GatewayMethodEditor("BrainTree PayPal OneTime Payment Method Editor", "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.paymentmethod.addedit.html")]
    public class PayPalOneTimeTransactionPaymentGatewayMethod : BraintreePaymentGatewayMethodBase, IPayPalOneTimeTransactionPaymentGatewayMethod
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
        }

        protected override IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            throw new System.NotImplementedException();
        }

        protected override IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
        {
            throw new System.NotImplementedException();
        }

        protected override IPaymentResult ProcessPayment(
            IInvoice invoice,
            TransactionOption option,
            decimal amount,
            string token,
            string email = "")
        {
            throw new System.NotImplementedException();
        }
    }
}