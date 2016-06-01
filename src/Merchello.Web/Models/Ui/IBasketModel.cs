namespace Merchello.Web.Models.Ui
{
    /// <summary>
    /// Defines a basket UI component.
    /// </summary>
    /// <typeparam name="TBasketItemModel">
    /// The type of the basket item.
    /// </typeparam>
    public interface IBasketModel<TBasketItemModel> : IItemCacheModel<TBasketItemModel>
        where TBasketItemModel : class, ILineItemModel, new()
    {
        /// <summary>
        /// Gets or sets a value indicating whether the wish list is enabled.
        /// </summary>
        bool WishListEnabled { get; set; }
    }
}