namespace Merchello.Providers.Payment
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Constants segment for the PayPal provider.
    /// </summary>
    internal static partial class Constants
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
            public const string PayPalPaymentGatewayProviderKey = "C87B8EF7-566E-4D2D-90F8-84A0023E75A9";

            /// <summary>
            /// PayPal ExtendedData keys.
            /// </summary>
            public static class ExtendedDataKeys
            {
                /// <summary>
                /// Gets the processor settings.
                /// </summary>
                public static string ProcessorSettings
                {
                    get
                    {
                        return "paypalProcessorSettings";
                    }
                }

                /// <summary>
                /// Gets the transaction id.
                /// </summary>
                public static string TransactionId
                {
                    get
                    {
                        return "paypalTransactionId";
                    }
                }

                /// <summary>
                /// Gets the authorization id.
                /// </summary>
                public static string AuthorizationId
                {
                    get
                    {
                        return "paypalAuthorizationId";
                    }
                }

                /// <summary>
                /// Gets the amount currency id.
                /// </summary>
                public static string AmountCurrencyId
                {
                    get
                    {
                        return "paypalAmountCurrencyId";
                    }
                }

                /// <summary>
                /// Gets the return URL.
                /// </summary>
                public static string ReturnUrl
                {
                    get
                    {
                        return "paypalReturnUrl";
                    }
                }

                /// <summary>
                /// Gets the cancel URL.
                /// </summary>
                public static string CancelUrl
                {
                    get
                    {
                        return "paypalCancelUrl";
                    }
                }

                /// <summary>
                /// Gets the payment authorized.
                /// </summary>
                public static string PaymentAuthorized
                {
                    get
                    {
                        return "paypalPaymentAuthorized";
                    }
                }

                /// <summary>
                /// Gets the payment captured.
                /// </summary>
                public static string PaymentCaptured
                {
                    get
                    {
                        return "paypalPaymentCaptured";
                    }
                }

                /// <summary>
                /// Gets the capture amount.
                /// </summary>
                public static string CaptureAmount
                {
                    get
                    {
                        return "CaptureAmount";
                    }
                }

                /// <summary>
                /// Gets the login id.
                /// </summary>
                public static string LoginId
                {
                    get
                    {
                        return "paypalLoginId";
                    }
                }

                /// <summary>
                /// Gets the transaction key.
                /// </summary>
                public static string TransactionKey
                {
                    get
                    {
                        return "paypalTranKey";
                    }
                }

                /// <summary>
                /// Gets the cc last four.
                /// </summary>
                public static string CcLastFour
                {
                    get
                    {
                        return "paypalCCLastFour";
                    }
                }

                /// <summary>
                /// Gets the authorize declined result.
                /// </summary>
                public static string AuthorizeDeclinedResult
                {
                    get
                    {
                      return "paypalAuthorizeDeclined";
                    }
                }

                /// <summary>
                /// Gets the authorization transaction code.
                /// </summary>
                public static string AuthorizationTransactionCode
                {
                    get
                    {
                        return "paypalAuthorizeTransactionCode";
                    }
                }

                /// <summary>
                /// Gets the authorization transaction result.
                /// </summary>
                public static string AuthorizationTransactionResult
                {
                    get
                    {
                        return "paypalAuthorizeTransactionResult";
                    }
                }

                /// <summary>
                /// Gets the AVS result.
                /// </summary>
                public static string AvsResult
                {
                    get
                    {
                        return "paypalAvsResult";
                    }
                }

                /// <summary>
                /// Gets the capture declined result.
                /// </summary>
                public static string CaptureDeclinedResult
                {
                    get
                    {
                        return "paypalCaptureDeclined";
                    }
                }

                /// <summary>
                /// Gets the capture transaction code.
                /// </summary>
                public static string CaptureTransactionCode
                {
                    get
                    {
                        return "paypalCaptureTransactionCode";
                    }
                }

                /// <summary>
                /// Gets the capture transaction result.
                /// </summary>
                public static string CaptureTransactionResult
                {
                    get
                    {
                        return "paypalCaptureTransactionResult";
                    }
                }

                /// <summary>
                /// Gets the refund declined result.
                /// </summary>
                public static string RefundDeclinedResult
                {
                    get
                    {
                        return "paypalRefundDeclined";
                    }
                }

                /// <summary>
                /// Gets the void declined result.
                /// </summary>
                public static string VoidDeclinedResult
                {
                    get
                    {
                        return "paypalVoidDeclined";
                    }
                } 
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
                /// Gets the article by SKU path.
                /// </summary>
                /// <remarks>
                /// TODO RSS What is this?
                /// </remarks>
                public static string ArticleBySkuPath
                {
                    get
                    {
                        return "ArticleBySkuPath";
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
