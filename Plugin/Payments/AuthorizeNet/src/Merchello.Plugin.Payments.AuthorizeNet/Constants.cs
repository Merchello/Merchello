namespace Merchello.Plugin.Payments.AuthorizeNet
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The constants.
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// The extended data keys.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static class ExtendedDataKeys
        {
            /// <summary>
            /// The processor settings.
            /// </summary>
            public static string ProcessorSettings = "authorizeNetProcessorSettings";

            /// <summary>
            /// The login id.
            /// </summary>
            public static string LoginId = "authorizeNetLoginId";

            /// <summary>
            /// The transaction key.
            /// </summary>
            public static string TransactionKey = "authorizeNetTranKey";

            /// <summary>
            /// The cc last four.
            /// </summary>
            public static string CcLastFour = "authorizeNetCCLastFour";

            /// <summary>
            /// The authorize declined result.
            /// </summary>
            public static string AuthorizeDeclinedResult = "authorizeNetAuthorizeDeclined";

            /// <summary>
            /// The authorization transaction code.
            /// </summary>
            public static string AuthorizationTransactionCode = "authorizeNetAuthorizeTransactionCode";

            /// <summary>
            /// The authorization transaction result.
            /// </summary>
            public static string AuthorizationTransactionResult = "authorizeNetAuthorizeTransactionResult";

            /// <summary>
            /// The avs result.
            /// </summary>
            public static string AvsResult = "authorizeNetAvsResult";

            /// <summary>
            /// The capture declined result.
            /// </summary>
            public static string CaptureDeclinedResult = "authorizeNetCaptureDeclined";

            /// <summary>
            /// The caputure transaction code.
            /// </summary>
            public static string CaputureTransactionCode = "authorizeNetCaptureTransactionCode";

            /// <summary>
            /// The capture transaction result.
            /// </summary>
            public static string CaptureTransactionResult = "authorizeNetCaptureTransactionResult";

            /// <summary>
            /// The refund declined result.
            /// </summary>
            public static string RefundDeclinedResult = "authorizeNetRefundDeclined";

            /// <summary>
            /// The void declined result.
            /// </summary>
            public static string VoidDeclinedResult = "authorizeNetVoidDeclined";
        }
    }
}