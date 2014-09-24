namespace Merchello.Plugin.Payments.Braintree
{
    using System;

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
            get { return new Guid("D143E0F6-98BB-4E0A-8B8C-CE9AD91B0969"); }
        }

        /// <summary>
        /// Gets the transaction channel.
        /// </summary>
        public static string TransactionChannel
        {
            get { return "MerchelloBraintreePaymentProvider"; }
        }

        /// <summary>
        /// Constant ExtendedDataCollection keys
        /// </summary>
        public static class ExtendedDataKeys
        {
            /// <summary>
            /// Gets the key for the BraintreeProviderSettings serialization.
            /// </summary>
            public static string BraintreeProviderSettings
            {
                get
                {
                    return "brainTreeProviderSettings";
                }
            }
        }
    }
}