namespace Merchello.Plugin.Payments.PayPal.Models
{
	public class PayPalProcessorSettings
	{
		public string AccountId { get; set; }
		public string ApiUsername { get; set; }
		public string ApiPassword { get; set; }
		public string ApiSignature { get; set; }

        public string ApiVersion
        {
            get { return PayPalPaymentProcessor.ApiVersion; }
        }
	}
}
