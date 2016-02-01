namespace Merchello.Bazaar.Models
{
    /// <summary>
    /// The updated sale summary.
    /// </summary>
    public partial class UpdatedSaleSummary
    {
        /// <summary>
        /// Gets or sets the total label.
        /// </summary>
        public string TotalLabel { get; set; }

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
    }
}