namespace Merchello.FastTrack.Models.Payment
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class PurchaseOrderPaymentModel : FastTrackPaymentModel
    {
        /// <summary>
        /// Gets or sets the PO number.
        /// </summary>
        [Required]
        [DisplayName(@"Purchase Order Number")]
        public string PurchaseOrderNumber { get; set; }
    }
}
