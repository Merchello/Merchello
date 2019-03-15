namespace Merchello.FastTrack.Models.Payment
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Merchello.Web.Store.Models;

    /// <summary>
    /// Represents a purchase order payment.
    /// </summary>
    public class BankTransferModel : FastTrackPaymentModel, IPurchaseOrderPaymentModel
    {
        /// <summary>
        /// Gets or sets the PO number.
        /// </summary>
        [Required]
        [DisplayName(@"Purchase Order Number")]
        public string PurchaseOrderNumber { get; set; }
    }
}
