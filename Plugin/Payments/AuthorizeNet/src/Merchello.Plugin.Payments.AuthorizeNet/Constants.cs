namespace Merchello.Plugin.Payments.AuthorizeNet
{
    public class Constants
    {
        public static class ExtendedDataKeys
        {
            public static string ProcessorSettings = "authorizeNetProcessorSettings";
            public static string LoginId = "authorizeNetLoginId";
            public static string TransactionKey = "authorizeNetTranKey";

            public static string DeclinedResult = "authorizeNetDeclined";
            public static string AuthorizationTransactionCode = "authorizeNetTransactionCode";
            public static string AuthorizationTransactionResult = "authorizeNetTransactionResult";
            public static string AvsResult = "authorizeNetAvsResult";
        }
    }
}