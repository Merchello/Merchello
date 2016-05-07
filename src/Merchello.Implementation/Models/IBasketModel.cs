namespace Merchello.Implementation.Models
{
    /// <summary>
    /// Defines a basket UI component.
    /// </summary>
    public interface IBasketModel : IMerchelloImplementationModel
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