namespace Merchello.Core
{
    /// <summary>
    /// Merchello constants.
    /// </summary>
    public static partial class Constants
    {
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
            /// Gets the configuration key for the checkout manager invoice creation.
            /// </summary>
            public static string CheckoutManagerInvoiceCreate
            {
                get
                {
                    return "CheckoutManagerInvoiceCreate";
                }
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