namespace Merchello.Plugin.Payments.PayPal.Models
{
	public class PayPalProcessorSettings
	{

		public string AccountId { get; set; }
		public string ApiUsername { get; set; }
		public string ApiPassword { get; set; }
		public string ApiSignature { get; set; }
		
		public bool LiveMode { get; set; }

		public string ArticleBySkuPath { get; set; }
		public string ReturnUrl { get; set; }
		public string CancelUrl { get; set; }
<<<<<<< HEAD
=======

		public bool CaptureFunds { get; set; }
>>>>>>> d2c22cd63ea00bc79c74f2a720da7a25499daa62

        public string ApiVersion
        {
            get { return PayPalPaymentProcessor.ApiVersion; }
        }
	}
}
