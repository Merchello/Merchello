namespace Merchello.Bazaar.Models
{
    using Merchello.Core.Models;

    using Umbraco.Core.Models;

    /// <summary>
    /// The basket table model.
    /// </summary>
    public partial class BasketTableModel : ItemCollectionTable
    {
        /// <summary>
        /// Gets or sets the checkout page.
        /// </summary>
        public IPublishedContent CheckoutPage { get; set; }

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

        /// <summary>
        /// Gets or sets a value indicating whether show wish list buttons.
        /// </summary>
        public bool ShowWishList { get; set; }
    }
}