namespace Merchello.Web.Store.Models
{
    using Merchello.Web.Models.Ui;

    /// <summary>
    /// Represents a Purchase Order Payment Model.
    /// </summary>
    public interface IBankTransferModel : ICheckoutPaymentModel
    {
        /// <summary>
        /// Gets or sets the purchase order number.
        /// </summary>
        string PurchaseOrderNumber { get; set; }
    }
}