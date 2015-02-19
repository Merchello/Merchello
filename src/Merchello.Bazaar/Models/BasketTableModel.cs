namespace Merchello.Bazaar.Models
{
    using Merchello.Core.Models;

    using Umbraco.Core.Models;

    /// <summary>
    /// The basket table model.
    /// </summary>
    public class BasketTableModel
    {
        /// <summary>
        /// Gets or sets the continue shopping page.
        /// </summary>
        public IPublishedContent ContinueShoppingPage { get; set; }

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
    }
}