namespace Merchello.Core
{
    /// <summary>
    /// Merchello constants.
    /// </summary>
    public partial class Constants
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
            /// Gets the configruation key for the default shipment rate quote.
            /// </summary>
            public static string DefaultShipmentRateQuote
            {
                get { return "DefaultShipmentRateQuote"; }   
            }

            /// <summary>
            /// Gets the configruation key for the default invoice tax rate quote
            /// </summary>
            public static string DefaultInvoiceTaxRateQuote
            {
                get { return "DefaultInvoiceTaxRateQuote";  }
            }
        }

        /// <summary>
        /// Default task chains
        /// </summary>
        public static class TaskChainAlias
        {
            /// <summary>
            /// Gets the configuration key for the sales preparation invoice create.
            /// </summary>
            public static string SalesPreparationInvoiceCreate
            {
                get { return "SalesPreparationInvoiceCreate"; }
            }

            /// <summary>
            /// Gets the configuration key for the order preparation order create.
            /// </summary>
            public static string OrderPreparationOrderCreate
            {
                get { return "OrderPreparationOrderCreate"; }   
            }

            /// <summary>
            /// Gets the configuration key for the preparation shipment create.
            /// </summary>
            public static string OrderPreparationShipmentCreate
            {
                get { return "OrderPreparationShipmentCreate"; }
            }
        }
    }
}