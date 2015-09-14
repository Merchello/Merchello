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
            /// Gets the item cache validation.
            /// </summary>
            public static string ItemCacheValidation
            {
                get
                {
                    return "ItemCacheValidation";
                }
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

            /// <summary>
            /// Gets the configuration key for the MerchelloHelper product data modifiers.
            /// </summary>
            public static string MerchelloHelperProductDataModifiers
            {
                get { return "MerchelloHelperProductDataModifiers"; }
            }

            /// <summary>
            /// Gets the copy product.
            /// </summary>
            public static string CopyProduct
            {
                get
                {
                    return "CopyProduct";
                }
            }
        }
    }
}