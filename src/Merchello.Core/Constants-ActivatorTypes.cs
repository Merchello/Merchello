namespace Merchello.Core
{
    public partial class Constants
    {
        public static class StrategyTypeAlias
        {
            public static string DefaultBasketPackaging = "DefaultBasketPackaging";
            public static string DefaultShipmentRateQuote = "DefaultShipmentRateQuote";
            public static string DefaultInvoiceTaxRateQuote = "DefaultInvoiceTaxRateQuote";
        }

        public static class TaskChainAlias
        {
            public static string CheckoutInvoiceCreate = "CheckoutInvoiceCreate";
            public static string InvoiceTaxRateQuote = "InvoiceTaxRateQuote";
        }
    }
}