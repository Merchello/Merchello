using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Plugin.Payments.PayPal.Provider
{
	/// <summary>
	/// Represents a PayPal Payment Method
	/// </summary>
    [GatewayMethodUi("PayPalPayment")]
    public class PayPalPaymentGatewayMethod : PaymentGatewayMethodBase
	{
		private readonly PayPalPaymentProcessor _processor;

		public PayPalPaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, ExtendedDataCollection providerExtendedData) 
            : base(gatewayProviderService, paymentMethod)
        {
			_processor = new PayPalPaymentProcessor(providerExtendedData.GetProcessorSettings());
        }

		protected override IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
		{
			return ProcessPayment(invoice, args);
		}

		private IPaymentResult ProcessPayment(IInvoice invoice, ProcessorArgumentCollection args)
		{
			var payment = GatewayProviderService.CreatePayment(PaymentMethodType.Cash, invoice.Total, PaymentMethod.Key);
			payment.CustomerKey = invoice.CustomerKey;
			payment.Authorized = false;
			payment.Collected = false;
			payment.PaymentMethodName = "PayPal";
			GatewayProviderService.Save(payment);

			var result = _processor.ProcessPayment(invoice, payment, args);
			GatewayProviderService.Save(payment);

			GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, string.Format("To show promise of a {0} payment", PaymentMethod.Name), 0);

			return result;
		}

		protected override IPaymentResult PerformCapturePayment(IInvoice invoice, IPayment payment, decimal amount,
														ProcessorArgumentCollection args)
		{
			var token = args["token"];
			var payerId = args["PayerID"];

			var result = _processor.CompletePayment(invoice, payment, token, payerId);

			GatewayProviderService.Save(payment);

			// TODO
			GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "PayPal Payment", payment.Amount);

			/*
			if (!result.Payment.Success)
			{
				GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied,
					result.Payment.Exception.Message, 0);
			}
			else
			{
				GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit,
					payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.CaptureTransactionResult), amount);
			}
			*/

			return result;
		}

		protected override IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
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
