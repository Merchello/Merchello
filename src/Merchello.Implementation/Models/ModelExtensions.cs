namespace Merchello.Implementation.Models
{
    using System.Linq;

    /// <summary>
    /// Extensions methods for implementation models.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Gets the total price of a basket line item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The total price.
        /// </returns>
        public static decimal Total(this ILineItemModel item)
        {
            return item.Quantity * item.Amount;
        }

        /// <summary>
        /// Gets the total price of the basket.
        /// </summary>
        /// <typeparam name="TBasketItemModel">
        /// The type of basket item
        /// </typeparam>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <returns>
        /// The total price.
        /// </returns>
        public static decimal Total<TBasketItemModel>(this IBasketModel<TBasketItemModel> basket)
            where TBasketItemModel : class, ILineItemModel, new()
        {
            return basket.Items.Sum(x => x.Total());
        }
    }
}