using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Plugin.Payments.AuthorizeNet.Provider
{
    /// <summary>
    /// Represents an AuthorizeNet Payment Method
    /// </summary>
    public class AuthorizeNetPaymentGatewayMethod : PaymentGatewayMethodBase, IAuthorizeNetPaymentGatewayMethod
    {
        public AuthorizeNetPaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod) 
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

        protected override IPaymentResult PerformRefundPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args)
        {
            throw new System.NotImplementedException();
        }

        protected override IPaymentResult PerformVoidPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args)
        {
            throw new System.NotImplementedException();
        }
    }
}