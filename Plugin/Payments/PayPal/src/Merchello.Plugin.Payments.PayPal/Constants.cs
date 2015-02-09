namespace Merchello.Plugin.Payments.PayPal
{
	public class Constants
	{

		public const string PayPalPaymentGatewayProviderKey = "4E9D52B5-65A2-4F23-89D6-8E83500D4137";

		public static class ExtendedDataKeys
		{
			public static string ProcessorSettings = "paypalProcessorSettings";

			public static string TransactionId = "paypalTransactionId";

			public static string AuthorizationId = "paypalAuthorizationId";
			public static string AmountCurrencyId = "paypalAmountCurrencyId";

			public static string ReturnUrl = "paypalReturnUrl";
			public static string CancelUrl = "paypalCancelUrl";

			public static string PaymentAuthorized = "paypalPaymentAuthorized";
			public static string PaymentCaptured = "paypalPaymentCaptured";

			public static string CaptureAmount = "CaptureAmount";

			/*
			public static string LoginId = "paypalLoginId";
			public static string TransactionKey = "paypalTranKey";

			public static string CcLastFour = "paypalCCLastFour";

			public static string AuthorizeDeclinedResult = "paypalAuthorizeDeclined";
			public static string AuthorizationTransactionCode = "paypalAuthorizeTransactionCode";
			public static string AuthorizationTransactionResult = "paypalAuthorizeTransactionResult";
			public static string AvsResult = "paypalAvsResult";

			public static string CaptureDeclinedResult = "paypalCaptureDeclined";
			public static string CaputureTransactionCode = "paypalCaptureTransactionCode";
			public static string CaptureTransactionResult = "paypalCaptureTransactionResult";

			public static string RefundDeclinedResult = "paypalRefundDeclined";
			public static string VoidDeclinedResult = "paypalVoidDeclined";
			*/
		}

		public static class ProcessorArgumentsKeys
		{
			public static string ReturnUrl = "ReturnUrl";

			public static string CancelUrl = "CancelUrl";

			public static string ArticleBySkuPath = "ArticleBySkuPath";

			internal static string internalTokenKey = "internalToken";
			internal static string internalPayerIDKey = "internalPayerID";
			internal static string internalPaymentKeyKey = "internalPaymentKey";

		}

	}
}
