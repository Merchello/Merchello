namespace Merchello.Bazaar.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The confirmation pre sale summary.
    /// </summary>
    public class SalePreparationSummary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SalePreparationSummary"/> class.
        /// </summary>
        public SalePreparationSummary()
        {
            Messages = new List<string>();
        }

        /// <summary>
        /// Gets or sets the total label.
        /// </summary>
        public string TotalLabel { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public IEnumerable<BasketLineItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the item total.
        /// </summary>
        public string ItemTotal { get; set; }

        /// <summary>
        /// Gets or sets the shipping total.
        /// </summary>
        public string ShippingTotal { get; set; }

        /// <summary>
        /// Gets or sets the tax total.
        /// </summary>
        public string TaxTotal { get; set; }

        /// <summary>
        /// Gets or sets the discounts total.
        /// </summary>
        public string DiscountsTotal { get; set; }

        /// <summary>
        /// Gets or sets the invoice total.
        /// </summary>
        public string InvoiceTotal { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show the apply offer code form.
        /// </summary>
        public bool ShowApplyOfferCodeForm { get; set; }


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
    }
}