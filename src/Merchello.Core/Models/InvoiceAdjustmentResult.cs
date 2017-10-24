namespace Merchello.Core.Models
{
    /// <summary>
    ///     A result from adjusting an existing invoice
    /// </summary>
    public class InvoiceAdjustmentResult
    {
        /// <summary>
        ///     Was the overall adjustment successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        ///     A return message, could be an error
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Was a order adjusted in the process
        /// </summary>
        public int AmountOrdersAdjusted { get; set; }

        /// <summary>
        ///     Was a shipment adjusted in the process
        /// </summary>
        public bool AmountShipmentsAdjusted { get; set; }

        /// <summary>
        ///     The type of adjustment
        /// </summary>
        public InvoiceAdjustmentType InvoiceAdjustmentType { get; set; }
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
        ///     Increasing the quantity of an existing product line item
        /// </summary>
        IncreaseProductQuantity,

        /// <summary>
        ///     Decreasing the quantity of an existing product line item
        /// </summary>
        DecreaseProductQuantity,

        /// <summary>
        ///     Deleting a product line item
        /// </summary>
        DeleteProduct,

        /// <summary>
        ///     General Adjustment
        /// </summary>
        General
    }
}