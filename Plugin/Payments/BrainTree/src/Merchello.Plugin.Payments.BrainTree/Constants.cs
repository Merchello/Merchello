namespace Merchello.Plugin.Payments.Braintree
{
    using System;
    using global::Braintree;

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
                    return "braintreeProviderSettings";
                }
            }

            /// <summary>
            /// Gets the key key for the serialized braintree <see cref="Transaction"/>.
            /// </summary>
            public static string BraintreeTransaction
            {
                get { return "braintreeTransaction";  }
            }
        }

        /// <summary>
        /// Braintree processor arguments.
        /// </summary>
        public static class ProcessorArguments
        {
            /// <summary>
            /// Gets the key for the payment method nonce.
            /// </summary>
            public static string PaymentMethodNonce
            {
                get { return "nonce-from-the-client"; }
            }

            /// <summary>
            /// Gets the key for the payment method token.
            /// </summary>
            public static string PaymentMethodToken
            {
                get { return "the_token";  }
            }

            /// <summary>
            /// Gets the key for a subscription plan id.
            /// </summary>
            public static string PlanId
            {
                get { return "planId"; }
            }
        }
    }
}