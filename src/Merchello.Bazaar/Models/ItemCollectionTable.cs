namespace Merchello.Bazaar.Models
{
    using Umbraco.Core.Models;

    /// <summary>
    /// The item collection table.
    /// </summary>
    public abstract class ItemCollectionTable
    {
        /// <summary>
        /// Gets or sets the continue shopping page.
        /// </summary>
        public IPublishedContent ContinueShoppingPage { get; set; }

        /// <summary>
        /// Gets or sets the basket page id.
        /// </summary>
        public int BasketPageId { get; set; }

        /// <summary>
        /// Gets or sets the wish list page id.
        /// </summary>
        public int WishListPageId { get; set; }
    }
}