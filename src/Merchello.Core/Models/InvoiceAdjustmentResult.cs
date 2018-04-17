namespace Merchello.Core.Models
{
    /// <summary>
    ///     A result from adjusting an existing invoice
    /// </summary>
    public class InvoiceAdjustmentResult
    {
        #region Ctor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="invoiceLineItemType">
        /// Allows a string to be passed into preset the InvoiceLineItemType
        /// </param>
        public InvoiceAdjustmentResult(string invoiceLineItemType)
        {
            switch (invoiceLineItemType)
            {
                case "Custom":
                    InvoiceLineItemType = InvoiceLineItemType.Custom;
                    break;
                default:
                    InvoiceLineItemType = InvoiceLineItemType.Product;
                    break;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InvoiceAdjustmentResult()
        {

        } 

        #endregion

        /// <summary>
        ///     Was the overall adjustment successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        ///     A return message, could be an error
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     The type of adjustment
        /// </summary>
        public InvoiceAdjustmentType InvoiceAdjustmentType { get; set; }

        /// <summary>
        /// The type of 
        /// </summary>
        public InvoiceLineItemType InvoiceLineItemType { get; set; }
    }

    /// <summary>
    ///     The type of line item being edited
    /// </summary>
    public enum InvoiceLineItemType
    {
        /// <summary>
        ///     standard product line item
        /// </summary>
        Product,

        /// <summary>
        ///     Custom line item
        /// </summary>
        Custom
    }

    /// <summary>
    ///     The type of adjustment for the invoice
    /// </summary>
    public enum InvoiceAdjustmentType
    {
        /// <summary>
        ///     Adding product(s) to an invoice
        /// </summary>
        AddProducts,

        /// <summary>
        ///     Deleting a product line item
        /// </summary>
        DeleteProduct,

        /// <summary>
        /// Used for custom products, where the name, qty, sku or price has been adjusted
        /// </summary>
        UpdateProductDetails,

        /// <summary>
        ///     General Adjustment
        /// </summary>
        General
    }
}