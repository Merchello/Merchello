namespace Merchello.Web.Models.Ui
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;

    /// <summary>
    /// Extensions methods for implementation models.
    /// </summary>
    public static class UiModelExtensions
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

        /// <summary>
        /// Gets the option choice pairs for a variant added as a basket item.
        /// </summary>
        /// <param name="lineItem">
        /// The <see cref="ILineItem"/>.
        /// </param>
        /// <returns>
        /// The option choice pairs for a variant added to the basket.
        /// </returns>
        public static Dictionary<string, string> GetProductOptionChoicePairs(this ILineItem lineItem)
        {
            var values =
                lineItem.ExtendedData.GetValue<Dictionary<string, string>>(Core.Constants.ExtendedDataKeys.BasketItemCustomerChoice);

            return values ?? new Dictionary<string, string>();
        }
    }
}