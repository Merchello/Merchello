namespace Merchello.Providers
{
    using System;

    /// <summary>
    /// Constants segment for the PayPal provider.
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// PayPal constants.
        /// </summary>
        public static class PayPal
        {
            /// <summary>
            /// The PayPal payment gateway provider key.
            /// </summary>
            /// <remarks>
            /// This has changed from the original plugin to hopefully avoid conflicts
            /// </remarks>
            public const string GatewayProviderKey = "C87B8EF7-566E-4D2D-90F8-84A0023E75A9";


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
            /// Payment codes for the PayPal provider.
            /// </summary>
            public static class PaymentCodes
            {
                /// <summary>
                /// Gets the Express Checkout Payment code.
                /// </summary>
                public const string ExpressCheckout = "PayPalExpress";
            }

            /// <summary>
            /// PayPal ExtendedData keys.
            /// </summary>
            public static class ExtendedDataKeys
            {
                /// <summary>
                /// Gets the processor settings.
                /// </summary>
                public const string ProviderSettings = "paypalprovidersettings";

                /// <summary>
                /// The pay pal express transaction key.
                /// </summary>
                public const string PayPalExpressTransaction = "paypalexpresstransaction";
            }

            /// <summary>
            /// The processor arguments keys.
            /// </summary>
            public static class ProcessorArgumentsKeys
            {
                /// <summary>
                /// Gets the return URL.
                /// </summary>
                public static string ReturnUrl
                {
                    get
                    {
                        return "ReturnUrl";
                    }
                }

                /// <summary>
                /// Gets the cancel url.
                /// </summary>
                public static string CancelUrl
                {
                    get
                    {
                        return "CancelUrl";
                    }
                }

                /// <summary>
                /// Gets the product content slug.
                /// </summary>
                public static string ProductContentSlug
                {
                    get
                    {
                        return "productContentSlug";
                    }
                }

                /// <summary>
                /// Gets the internal token key.
                /// </summary>
                internal static string InternalTokenKey
                {
                    get
                    {
                        return "internalToken";
                    }
                }

                /// <summary>
                /// Gets the internal payer id key.
                /// </summary>
                internal static string InternalPayerIDKey
                {
                    get
                    {
                        return "internalPayerID";
                    }
                }

                /// <summary>
                /// Gets the internal payment key key.
                /// </summary>
                internal static string InternalPaymentKeyKey
                {
                    get
                    {
                        return "internalPaymentKey";
                    }
                } 

            }
        }
    }
}
