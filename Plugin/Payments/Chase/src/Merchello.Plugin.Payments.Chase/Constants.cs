namespace Merchello.Plugin.Payments.Chase
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
            public static string ProcessorSettings = "chaseProcessorSettings";

            /// <summary>
            /// The login id.
            /// </summary>
            public static string LoginId = "chaseLoginId";

            /// <summary>
            /// The transaction key.
            /// </summary>
            public static string TransactionKey = "chaseTranKey";

            /// <summary>
            /// The cc last four.
            /// </summary>
            public static string CcLastFour = "chaseCCLastFour";

            /// <summary>
            /// The authorize declined result.
            /// </summary>
            public static string AuthorizeDeclinedResult = "chaseAuthorizeDeclined";

            /// <summary>
            /// The authorization transaction code.
            /// </summary>
            public static string AuthorizationTransactionCode = "chaseAuthorizeTransactionCode";

            /// <summary>
            /// The authorization transaction result.
            /// </summary>
            public static string AuthorizationTransactionResult = "chaseAuthorizeTransactionResult";

            /// <summary>
            /// The avs result.
            /// </summary>
            public static string AvsResult = "chaseAvsResult";

            /// <summary>
            /// The avs result.
            /// </summary>
            public static string Cvv2Result = "chaseCvv2Result";

            /// <summary>
            /// The authorization transaction result.
            /// </summary>
            public static string TransactionReferenceNumber = "txRefNum";

            /// <summary>
            /// The authorization transaction result.
            /// </summary>
            public static string TransactionReferenceIndex = "txRefIdx";

            /// <summary>
            /// The capture declined result.
            /// </summary>
            public static string CaptureDeclinedResult = "chaseCaptureDeclined";

            /// <summary>
            /// The caputure transaction code.
            /// </summary>
            public static string CaputureTransactionCode = "chaseCaptureTransactionCode";

            /// <summary>
            /// The capture transaction result.
            /// </summary>
            public static string CaptureTransactionResult = "chaseCaptureTransactionResult";

            /// <summary>
            /// The refund declined result.
            /// </summary>
            public static string RefundDeclinedResult = "chaseRefundDeclined";

            /// <summary>
            /// The void declined result.
            /// </summary>
            public static string VoidDeclinedResult = "chaseVoidDeclined";

            /// <summary>
            /// The void proc status result.
            /// </summary>
            public static string VoidProcStatus = "chaseVoidProcStatus";

        }
    }
}