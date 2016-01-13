namespace Merchello.Bazaar.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The model for the RedeemCouponOffer partial view.
    /// </summary>
    public partial class RedeemCouponOfferForm
    {
        /// <summary>
        /// Gets or sets the Bazaar theme name.
        /// </summary>
        public string ThemeName { get; set; }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        [Required(ErrorMessage = "Required")]
        public string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        public string CurrencySymbol { get; set; }
    }
}