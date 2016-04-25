namespace Merchello.Providers.Payment.PayPal.Provider
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Providers.Models;

    using Constants = Merchello.Providers.Constants;

    /// <summary>
	/// Represents a PayPal Payment Method
	/// </summary>
    [GatewayMethodUi("PayPalPayment")]
	[GatewayMethodEditor("PayPal Method Editor", "PayPal - Redirects to PayPal for Payment", "~/App_Plugins/Merchello.PayPal/paymentmethod.html")]
    public class PayPalPaymentGatewayMethod : RedirectPaymentMethodBase
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
			this._processor = new PayPalPaymentProcessor(providerExtendedData.GetPayPalProviderSettings());
        }

        /// <summary>
        /// Performs the authorize transaction.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        protected override IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
		{
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Performs the capture payment operation.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="payment">
        /// The payment.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
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
