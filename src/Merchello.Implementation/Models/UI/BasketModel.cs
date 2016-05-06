namespace Merchello.Web.Ui.Implementation
{
    /// <summary>
    /// A model to represent a basket in the UI.
    /// </summary>
    public class BasketModel : IBasketModel
    {
        /// <summary>
        /// Gets or sets the checkout page url.
        /// </summary>
        public string CheckoutPageUrl { get; set; }

        /// <summary>
        /// Gets or sets the continue shopping url.
        /// </summary>
        public string ContinueShoppingUrl { get; set; }

        /// <summary>
        /// Gets or sets the basket items.
        /// </summary>
        public IBasketItemModel[] Items { get; set; }
    }
}