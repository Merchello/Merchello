namespace Merchello.Plugin.Payments.Braintree.Provider
{
    using System;

    using global::Braintree;

    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Plugin.Payments.Braintree.Models;

    /// <summary>
    /// Represents the BraintreePaymentGatewayMethod
    /// </summary>
    [GatewayMethodUi("BrainTree.CreditCard")]
    [GatewayMethodEditor("BrainTree Payment Method Editor", "~/App_Plugins/Merchello.BrainTree/paymentmethod.html")]
    public class BraintreePaymentGatewayMethod : PaymentGatewayMethodBase, IBraintreePaymentGatewayMethod
    {
        /// <summary>
        /// The _settings.
        /// </summary>
        private readonly BraintreeGateway _gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentGatewayMethod"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        /// <param name="gateway">
        /// The <see cref="BraintreeGateway"/>
        /// </param>
        public BraintreePaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, BraintreeGateway gateway)
            : base(gatewayProviderService, paymentMethod)
        {
            if (gateway == null) throw new ArgumentNullException("gateway");
            _gateway = gateway;
        }

        protected override IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            throw new System.NotImplementedException();
        }

        protected override IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
        {
            throw new System.NotImplementedException();
        }

        protected override IPaymentResult PerformCapturePayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            throw new System.NotImplementedException();
        }

        protected override IPaymentResult PerformRefundPayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            throw new System.NotImplementedException();
        }

        protected override IPaymentResult PerformVoidPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args)
        {
            throw new System.NotImplementedException();
        }
    }
}