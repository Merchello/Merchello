namespace Merchello.Bazaar.Models
{
    using Merchello.Core.Models;

    /// <summary>
    /// The wish list table model.
    /// </summary>
    public class WishListTableModel
    {
        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        public ICurrency Currency { get; set; }

        /// <summary>
        /// Gets or sets the basket line items.
        /// </summary>
        public BasketLineItem[] Items { get; set; }

        /// <summary>
        /// Gets the total price.
        /// </summary>
        public decimal TotalPrice { get; internal set; } 
    }
}