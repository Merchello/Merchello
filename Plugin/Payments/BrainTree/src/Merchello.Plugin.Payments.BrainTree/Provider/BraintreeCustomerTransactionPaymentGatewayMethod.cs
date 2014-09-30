using Merchello.Plugin.Payments.Braintree.Services;

namespace Merchello.Plugin.Payments.Braintree.Provider
{
    using Core.Gateways.Payment;
    using Core.Models;
    using Core.Services;

    /// <summary>
    /// Represents a BraintreeCustomerTransactionPaymentGatewayMethod
    /// </summary>
    public class BraintreeCustomerTransactionPaymentGatewayMethod : PaymentGatewayMethodBase,  IBraintreeCustomerTransactionPaymentGatewayMethod
    {
        public BraintreeCustomerTransactionPaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, IBraintreeApiService braintreeApiService) 
            : base(gatewayProviderService, paymentMethod)
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

        protected override IPaymentResult PerformCapturePayment(IInvoice invoice, IPayment payment, decimal amount,
            ProcessorArgumentCollection args)
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