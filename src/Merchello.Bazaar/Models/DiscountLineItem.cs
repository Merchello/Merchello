namespace Merchello.Bazaar.Models
{
    using System;

    /// <summary>
    /// The discount line item.
    /// </summary>
    public partial class DiscountLineItem
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        public string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        public decimal Price { get; set; }
    }
}