namespace Merchello.Providers.Payment.PayPal.Provider
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Providers.Payment.PayPal;

    using Constants = Payment.Constants;

    /// <summary>
	/// Represents a PayPal Payment Method
	/// </summary>
    [GatewayMethodUi("PayPalPayment")]
	[GatewayMethodEditor("PayPal Method Editor", "PayPal - Redirects to PayPal for Payment", "~/App_Plugins/Merchello.PayPal/paymentmethod.html")]
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
			this._processor = new PayPalPaymentProcessor(providerExtendedData.GetProcessorSettings());
        }

		protected override IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
		{
			return this.InitializePayment(invoice, args, -1);
		}

        protected override IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
        {

			return this.InitializePayment(invoice, args, amount);
        }

		private IPaymentResult InitializePayment(IInvoice invoice, ProcessorArgumentCollection args, decimal captureAmount)
		{
			var payment = this.GatewayProviderService.CreatePayment(PaymentMethodType.CreditCard, invoice.Total, this.PaymentMethod.Key);
			payment.CustomerKey = invoice.CustomerKey;
			payment.Authorized = false;
			payment.Collected = false;
            payment.PaymentMethodName = "PayPal";
			payment.ExtendedData.SetValue(Constants.PayPal.ExtendedDataKeys.CaptureAmount, captureAmount.ToString(System.Globalization.CultureInfo.InvariantCulture));
			this.GatewayProviderService.Save(payment);

			var result = this._processor.InitializePayment(invoice, payment, args);

			if (!result.Payment.Success)
			{
				this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, "PayPal: request initialization error: " + result.Payment.Exception.Message, 0);
			}
			else
			{
				this.GatewayProviderService.Save(payment);
				this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "PayPal: initialized", 0);
			}

			return result;
		}

		protected override IPaymentResult PerformCapturePayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
		{
			
			var payedTotalList = invoice.AppliedPayments().Select(item => item.Amount).ToList();
			var payedTotal = (payedTotalList.Count == 0 ? 0 : payedTotalList.Aggregate((a, b) => a + b));
			var isPartialPayment = amount + payedTotal < invoice.Total;

			var result = this._processor.CapturePayment(invoice, payment, amount, isPartialPayment);
			//GatewayProviderService.Save(payment);
			
			if (!result.Payment.Success)
			{
				//payment.VoidPayment(invoice, payment.PaymentMethodKey.Value);
				this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, "PayPal: request capture error: " + result.Payment.Exception.Message, 0);
			}
			else
			{
				this.GatewayProviderService.Save(payment);
				this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "PayPal: captured", amount);
				//GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.CaptureTransactionResult), amount);
			}
			

			return result;
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
