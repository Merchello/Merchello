namespace Merchello.Bazaar.Models
{
    using Merchello.Core.Models;

    /// <summary>
    /// The wish list table model.
    /// </summary>
    public partial class WishListTableModel : ItemCollectionTable
    {
        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        public ICurrency Currency { get; set; }

        /// <summary>
        /// Gets or sets the basket line items.
        /// </summary>
        public BasketLineItem[] Items { get; set; }
    }
}