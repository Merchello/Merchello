namespace Merchello.Plugin.Payments.AuthorizeNet
{
    public class Constants
    {
        public static class ExtendedDataKeys
        {
            public static string ProcessorSettings = "authorizeNetProcessorSettings";
            public static string LoginId = "authorizeNetLoginId";
            public static string TransactionKey = "authorizeNetTranKey";

            public static string CcLastFour = "authorizeNetCCLastFour";

            public static string AuthorizeDeclinedResult = "authorizeNetAuthorizeDeclined";
            public static string AuthorizationTransactionCode = "authorizeNetAuthorizeTransactionCode";
            public static string AuthorizationTransactionResult = "authorizeNetAuthorizeTransactionResult";
            public static string AvsResult = "authorizeNetAvsResult";

            public static string CaptureDeclinedResult = "authorizeNetCaptureDeclined";
            public static string CaputureTransactionCode = "authorizeNetCaptureTransactionCode";
            public static string CaptureTransactionResult = "authorizeNetCaptureTransactionResult";

            public static string RefundDeclinedResult = "authorizeNetRefundDeclined";
            public static string VoidDeclinedResult = "authorizeNetVoidDeclined";
        }
    }
}