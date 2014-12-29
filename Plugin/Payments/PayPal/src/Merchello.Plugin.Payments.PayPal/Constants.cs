using System;

namespace Merchello.Plugin.Payments.PayPal
{
    /// <summary>
    /// The constants.
    /// </summary>
	public class Constants
	{
        /// <summary>
        /// Gets the gateway provider settings key.
        /// </summary>
        public static Guid GatewayProviderSettingsKey
        {
            get { return new Guid("4E9D52B5-65A2-4F23-89D6-8E83500D4137"); }
        }

		public static class ExtendedDataKeys
		{
			public static string ProcessorSettings = "paypalProcessorSettings";
            public static string OrderConfirmUrl = "OrderConfirmUrl";
            public static string AuthorizationId = "AuthorizationID";
            public static string AmountCurrencyId = "AmountCurrencyID";
            public static string TransactionId = "TransactionID";
		}
	}
}