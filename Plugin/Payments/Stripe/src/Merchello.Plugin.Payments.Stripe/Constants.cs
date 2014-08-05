namespace Merchello.Plugin.Payments.Stripe
{
    public class Constants
    {
        public static class ExtendedDataKeys
        {
            public static string ProcessorSettings = "stripeProcessorSettings";
            public static string LoginId = "stripeLoginId";
            public static string TransactionKey = "stripeTranKey";

            public static string CcLastFour = "stripeCCLastFour";

            public static string StripeChargeId = "stripeChargeId";
            public static string AuthorizeDeclinedResult = "stripeAuthorizeDeclined";

            public static string CaptureDeclinedResult = "stripeCaptureDeclined";
            public static string CaputureTransactionCode = "stripeCaptureTransactionCode";
            public static string CaptureTransactionResult = "stripeCaptureTransactionResult";

            public static string RefundDeclinedResult = "stripeRefundDeclined";
            public static string VoidDeclinedResult = "stripeVoidDeclined";
        }
    }
}