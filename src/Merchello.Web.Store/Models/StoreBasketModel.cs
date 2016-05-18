namespace Merchello.Web.Store.Models
{
    using Merchello.Web.Models.Ui;

    /// <summary>
    /// A model to represent a basket in the UI.
    /// </summary>
    public class StoreBasketModel : IBasketModel<StoreLineItemModel>
    {
        /// <summary>
        /// Gets or sets the basket items.
        /// </summary>
        public StoreLineItemModel[] Items { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the wish list is enabled.
        /// </summary>
        public bool WishListEnabled { get; set; }
    }
}