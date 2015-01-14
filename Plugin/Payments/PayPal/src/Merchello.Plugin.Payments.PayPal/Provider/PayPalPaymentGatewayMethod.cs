using System.Linq;

using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Services;

using Umbraco.Core;

namespace Merchello.Plugin.Payments.PayPal.Provider
{
	/// <summary>
	/// Represents a PayPal Payment Method
	/// </summary>
    [GatewayMethodUi("PayPalPayment")]
    public class PayPalPaymentGatewayMethod : PaymentGatewayMethodBase
	{
        /// <summary>
        /// The PayPal payment processor.
        /// </summary>
		private readonly PayPalPaymentProcessor _processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalPaymentGatewayMethod"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        /// <param name="providerExtendedData">
        /// The provider extended data.
        /// </param>
		public PayPalPaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, ExtendedDataCollection providerExtendedData) 
            : base(gatewayProviderService, paymentMethod)
        {
			_processor = new PayPalPaymentProcessor(providerExtendedData.GetProcessorSettings());
        }

        /// <summary>
        /// Does the actual work of creating and processing the payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="args">Any arguments required to process the payment.</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
		protected override IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
		{
            return ProcessPayment(invoice);
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
            return ProcessPayment(invoice);
        }

        private IPaymentResult ProcessPayment(IInvoice invoice)
		{
            var payment = GatewayProviderService.CreatePayment(PaymentMethodType.CreditCard, invoice.Total, PaymentMethod.Key);
			payment.CustomerKey = invoice.CustomerKey;
			payment.Authorized = false;
			payment.Collected = false;
			payment.PaymentMethodName = "PayPal";
            GatewayProviderService.Save(payment);

            var result = _processor.ExpressCheckout(invoice, payment);

            if (!result.Payment.Success)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, result.Payment.Exception.Message, 0);
            }
            else
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, string.Format("To show promise of a {0} payment", PaymentMethod.Name), 0);
            }

			return result;
		}

		protected override IPaymentResult PerformCapturePayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
		{
            var isPartialPayment = CalculateTotalOwed(invoice) + amount <= payment.Amount;
            var result = _processor.CapturePayment(invoice, payment, amount, isPartialPayment);

			GatewayProviderService.Save(payment);

            if (!result.Payment.Success)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, result.Payment.Exception.Message, 0);
            }
            else
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "PayPal Payment", amount);
            }

            return result;
		}

		protected override IPaymentResult PerformRefundPayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
		{
            var result = _processor.RefundPayment(invoice, payment);

            if (!result.Payment.Success)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied,
                   result.Payment.Exception.Message, 0);
                return result;
            }

            // use the overloaded AppliedPayments method here for testing if we don't have
            // a MerchelloContext
            foreach (var applied in payment.AppliedPayments(GatewayProviderService))
            {
                applied.TransactionType = AppliedPaymentType.Refund;
                applied.Amount = 0;
                applied.Description += " - Refunded";
                GatewayProviderService.Save(applied);
            }

            payment.Amount = 0;

            GatewayProviderService.Save(payment);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, false);
		}

		protected override IPaymentResult PerformVoidPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args)
		{
            payment.Voided = true;
            payment.Amount = 0;

            foreach (var applied in payment.AppliedPayments(GatewayProviderService))
            {
                applied.TransactionType = AppliedPaymentType.Void;
                applied.Amount = 0;
                applied.Description += " - Voided";
                GatewayProviderService.Save(applied);
            }

            GatewayProviderService.Save(payment);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, false);
		}

        private decimal CalculateTotalOwed(IInvoice invoice)
        {
            var applied = invoice.AppliedPayments(GatewayProviderService).ToArray();
            
            var owed =
                applied.Where(
                    x =>
                    x.TransactionType.Equals(AppliedPaymentType.Debit))
                    .Select(y => y.Amount)
                    .Sum()
                - applied.Where(
                    x =>
                    x.TransactionType.Equals(AppliedPaymentType.Credit))
                      .Select(y => y.Amount)
                      .Sum();

            return owed;
        }
	}
}
