namespace Merchello.Providers
{
    using System;

    using global::Braintree;

    /// <summary>
    /// Constants segment for Braintree constants
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// The Braintree Constants.
        /// </summary>
        public static class Braintree
        {
            /// <summary>
            /// The gateway provider key.
            /// </summary>
            public const string GatewayProviderKey = "D143E0F6-98BB-4E0A-8B8C-CE9AD91B0969";

            /// <summary>
            /// Gets the gateway provider settings key.
            /// </summary>
            public static Guid GatewayProviderSettingsKey
            {
                get
                {
                    return new Guid(GatewayProviderKey);
                }
            }

            /// <summary>
            /// Gets the transaction channel.
            /// </summary>
            public static string TransactionChannel
            {
                get
                {
                    return "MerchelloBraintreePaymentProvider";
                }
            }

            /// <summary>
            /// Braintree Provider PaymentCodes.
            /// </summary>
            public static class PaymentCodes
            {
                /// <summary>
                /// Gets the transaction.
                /// </summary>
                public const string Transaction = "BraintreeTransaction";

                /// <summary>
                /// Gets the vault transaction.
                /// </summary>
                public const string BraintreeVault = "BraintreetVaultTransaction";

                /// <summary>
                /// Gets a PayPal one time transaction.
                /// </summary>
                public const string PayPalOneTime = "PayPalOneTime";

                /// <summary>
                /// Get a PayPal vault transaction.
                /// </summary>
                public const string PayPalVault = "PalPalVault";

                /// <summary>
                /// Gets the record subscription transaction.
                /// </summary>
                public const string RecordSubscriptionTransaction = "BraintreeRecordSubscriptionTransaction";
            }

            /// <summary>
            /// Constant ExtendedDataCollection keys
            /// </summary>
            public static class ExtendedDataKeys
            {
                /// <summary>
                /// Gets the key for the BraintreeProviderSettings serialization.
                /// </summary>
                public const string ProviderSettings = "braintreeProviderSettings";

                /// <summary>
                /// Gets the key key for the serialized braintree <see cref="Transaction"/>.
                /// </summary>
                public static string BraintreeTransaction
                {
                    get
                    {
                        return "braintreeTransaction";
                    }
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
                    get
                    {
                        return "nonce-from-the-client";
                    }
                }

                /// <summary>
                /// Gets the key for the payment method token.
                /// </summary>
                public static string PaymentMethodToken
                {
                    get
                    {
                        return "the_token";
                    }
                }

                /// <summary>
                /// Gets the key for a subscription plan id.
                /// </summary>
                public static string PlanId
                {
                    get
                    {
                        return "planId";
                    }
                }

                /// <summary>
                /// Gets the billing address id.
                /// </summary>
                public static string BillingAddressId
                {
                    get
                    {
                        return "billingAddressId";
                    }
                }

                /// <summary>
                /// Gets the shipping address id.
                /// </summary>
                public static string ShippingAddressId
                {
                    get
                    {
                        return "shippingAddressId";
                    }
                }
            }
        }
    }
}