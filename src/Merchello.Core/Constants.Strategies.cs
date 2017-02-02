namespace Merchello.Core
{
    /// <summary>
    /// Constants for strategies.
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// Default strategies
        /// </summary>
        public static class StrategyTypeAlias
        {
            /// <summary>
            /// Gets the configuration key for the default packaging strategy
            /// </summary>
            public static string DefaultPackaging
            {
                get { return "DefaultPackaging"; }
            }

            /// <summary>
            /// Gets the configuration key for the default shipment rate quote.
            /// </summary>
            public static string DefaultShipmentRateQuote
            {
                get { return "DefaultShipmentRateQuote"; }
            }

            /// <summary>
            /// Gets the configuration key for the default invoice tax rate quote
            /// </summary>
            public static string DefaultInvoiceTaxRateQuote
            {
                get { return "DefaultInvoiceTaxRateQuote"; }
            }

            /// <summary>
            /// Gets the configuration key for the invoice itemization strategy.
            /// </summary>
            public static string InvoiceItemizationStrategy
            {
                get
                {
                    return "InvoiceItemizationStrategy";
                }
            }
        }
    }
}