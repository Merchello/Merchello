namespace Merchello.Web.Ui.Implementation
{
    using Merchello.Implementation.Models.UI;

    /// <summary>
    /// Defines a basket UI component.
    /// </summary>
    public interface IBasketModel : IStoreImplementationModel
    {
        /// <summary>
        /// Gets or sets the checkout page url.
        /// </summary>
        string CheckoutPageUrl { get; set; }

        /// <summary>
        /// Gets or sets the continue shopping url.
        /// </summary>
        string ContinueShoppingUrl { get; set; }

        /// <summary>
        /// Gets or sets the basket items.
        /// </summary>
        IBasketItemModel[] Items { get; set; } 
    }
}