using Merchello.Plugin.Payments.Braintree.Services;

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
        /// The <see cref="IBraintreeApiService"/>
        /// </summary>
        private readonly IBraintreeApiService _braintreeApiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentGatewayMethod"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        /// <param name="braintreeApiService">
        /// The braintree Api Service.
        /// </param>
        public BraintreePaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, IBraintreeApiService braintreeApiService)
            : base(gatewayProviderService, paymentMethod)
        {
            Mandate.ParameterNotNull(braintreeApiService, "braintreeApiService");

            _braintreeApiService = braintreeApiService;
        }

        protected override IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            // this will fail if MerchelloContext.Current is null
            var customer = invoice.Customer();
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