namespace Merchello.Bazaar.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The apply coupon form.
    /// </summary>
    public class ApplyCouponForm
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
        /// Gets or sets a value indicating whether the application was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the offer attempt messages
        /// </summary>
        public List<string> Messages { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public IEnumerable<DiscountLineItem> Items { get; set; }
    }
}