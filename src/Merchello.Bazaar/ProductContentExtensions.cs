namespace Merchello.Bazaar
{
    using Merchello.Bazaar.Models;
    using Merchello.Core.Models;
    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// The product content extensions.
    /// </summary>
    public static class ProductContentExtensions
    {
        /// <summary>
        /// Builds a <see cref="AddItemModel"/> from <see cref="IProductContent"/>.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="currency">
        /// The currency.
        /// </param>
        /// <param name="showWishList">
        /// The show wish list.
        /// </param>
        /// <returns>
        /// The <see cref="AddItemModel"/>.
        /// </returns>
        public static AddItemModel BuildAddItemModel(this IProductContent product, ICurrency currency, bool showWishList)
        {
            return new AddItemModel()
                {
                    Product = product.AsProductDisplay(),
                    ContentId = 0,
                    ContentUrl = product.Url,
                    BasketPageId = BazaarContentHelper.GetBasketContent().Id,
                    ShowWishList = showWishList,
                    WishListPageId = BazaarContentHelper.GetWishListContent().Id,
                    Currency = currency
                };
        }
    }
}