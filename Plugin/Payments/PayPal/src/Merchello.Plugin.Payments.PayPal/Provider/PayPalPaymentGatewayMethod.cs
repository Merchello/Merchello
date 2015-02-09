using System;
using System.Linq;
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
<<<<<<< HEAD
			return InitializePayment(invoice, args, -1);
		}

        protected override IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
        {

			return InitializePayment(invoice, args, amount);
        }

		private IPaymentResult InitializePayment(IInvoice invoice, ProcessorArgumentCollection args, decimal captureAmount)
=======
			return InitializePayment(invoice, args);
		}

		private IPaymentResult InitializePayment(IInvoice invoice, ProcessorArgumentCollection args)
>>>>>>> d2c22cd63ea00bc79c74f2a720da7a25499daa62
		{
			var payment = GatewayProviderService.CreatePayment(PaymentMethodType.CreditCard, invoice.Total, PaymentMethod.Key);
			payment.CustomerKey = invoice.CustomerKey;
			payment.Authorized = false;
			payment.Collected = false;
            payment.PaymentMethodName = "PayPal";
			payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.CaptureAmount, captureAmount.ToString(System.Globalization.CultureInfo.InvariantCulture));
			GatewayProviderService.Save(payment);

			var result = _processor.InitializePayment(invoice, payment, args);

			if (!result.Payment.Success)
			{
				GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, "PayPal: request initialization error: " + result.Payment.Exception.Message, 0);
			}
			else
			{
				GatewayProviderService.Save(payment);
				GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "PayPal: initialized", 0);
			}

			return result;
		}

		protected override IPaymentResult PerformCapturePayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
		{
			
			var payedTotalList = invoice.AppliedPayments().Select(item => item.Amount).ToList();
			var payedTotal = (payedTotalList.Count == 0 ? 0 : payedTotalList.Aggregate((a, b) => a + b));
			var isPartialPayment = amount + payedTotal < invoice.Total;

			var result = _processor.CapturePayment(invoice, payment, amount, isPartialPayment);
			//GatewayProviderService.Save(payment);
			
			if (!result.Payment.Success)
			{
				//payment.VoidPayment(invoice, payment.PaymentMethodKey.Value);
				GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, "PayPal: request capture error: " + result.Payment.Exception.Message, 0);
			}
			else
			{
				GatewayProviderService.Save(payment);
				GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "PayPal: captured", amount);
				//GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.CaptureTransactionResult), amount);
			}
			

			return result;
		}

<<<<<<< HEAD
=======
		protected override IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
		{
			string token;
			string payerId;
			string paymentKey;
			
			args.TryGetValue(Constants.ProcessorArgumentsKeys.internalTokenKey, out token);
			args.TryGetValue(Constants.ProcessorArgumentsKeys.internalPayerIDKey, out payerId);
			args.TryGetValue(Constants.ProcessorArgumentsKeys.internalPaymentKeyKey, out paymentKey);

			var paymentKeyGuid = new Guid(paymentKey);
			var payment = invoice.Payments().First(item => item.Key == paymentKeyGuid);

			var result = _processor.AuthorizePayment(invoice, payment, token, payerId);
			//GatewayProviderService.Save(payment);

			if (!result.Payment.Success)
			{
				GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, "PayPal: request capture authorization error: " + result.Payment.Exception.Message, 0);
			}
			else
			{
				GatewayProviderService.Save(payment);
				GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "PayPal: capture authorized", 0);
				//GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.CaptureTransactionResult), amount);
			}
			

			return result;
		}

>>>>>>> d2c22cd63ea00bc79c74f2a720da7a25499daa62
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
