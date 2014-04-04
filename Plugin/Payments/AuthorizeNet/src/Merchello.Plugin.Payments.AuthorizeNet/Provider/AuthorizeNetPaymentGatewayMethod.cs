using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.AuthorizeNet.Models;

namespace Merchello.Plugin.Payments.AuthorizeNet.Provider
{
    /// <summary>
    /// Represents an AuthorizeNet Payment Method
    /// </summary>
    public class AuthorizeNetPaymentGatewayMethod : PaymentGatewayMethodBase, IAuthorizeNetPaymentGatewayMethod
    {
        private readonly AuthorizeNetProcessorSettings _processorSettings;

        public AuthorizeNetPaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, ExtendedDataCollection providerExtendedData) 
            : base(gatewayProviderService, paymentMethod)
        {
            _processorSettings = providerExtendedData.GetProcessorSettings();
        }

        /// <summary>
        /// Does the actual work of creating and processing the payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="args">Any arguments required to process the payment.</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        protected override IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            return ProcessPayment(invoice, TransactionMode.Authorize, invoice.Total, args);
        }

        /// <summary>
        /// Does the actual work of authorizing and capturing a payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="amount">The amount to capture</param>
        /// <param name="args">Any arguments required to process the payment.</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        protected override IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
        {
            return ProcessPayment(invoice, TransactionMode.AuthorizeAndCapture, amount, args);
        }


        private IPaymentResult ProcessPayment(IInvoice invoice, TransactionMode transactionMode, decimal amount, ProcessorArgumentCollection args)
        {
            var payment = GatewayProviderService.CreatePayment(PaymentMethodType.CreditCard, invoice.Total, PaymentMethod.Key);
            payment.Authorized = false;
            payment.Collected = false;

            var processor = new AuthorizeNetPaymentProcessor(_processorSettings);

            var result = processor.ProcessPayment(invoice, payment, transactionMode, amount, args.AsCreditCardFormData());

            GatewayProviderService.Save(payment);

            if (!result.Payment.Success)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, result.Payment.Exception.Message, invoice.Total);
            }
            else
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit,
                    payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.AuthorizationTransactionResult),
                    invoice.Total);
            }

            return result;
        }

        protected override IPaymentResult PerformCapturePayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            var processor = new AuthorizeNetPaymentProcessor(_processorSettings);

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