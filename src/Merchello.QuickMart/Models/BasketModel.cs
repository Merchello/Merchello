namespace Merchello.QuickMart.Models
{
    using Merchello.Web.Models.Ui;

    /// <summary>
    /// A model to represent a basket in the UI.
    /// </summary>
    public class BasketModel : IBasketModel<BasketItemModel>
    {
        /// <summary>
        /// Gets or sets the basket items.
        /// </summary>
        public BasketItemModel[] Items { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the wish list is enabled.
        /// </summary>
        public bool WishListEnabled { get; set; }
    }
}