namespace Merchello.Web.Models.Ui
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
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
        /// Gets a value indicating whether the <see cref="ILineItemModel"/> is a shippable item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The value indicating whether the item is a shippable item.
        /// </returns>
        public static bool IsShippable(this ILineItemModel item)
        {
            var ed = item.ExtendedData.AsExtendedDataCollection();
            return item.LineItemType == LineItemType.Product &&
                   ed.ContainsProductVariantKey() &&
                   ed.GetShippableValue() &&
                   ed.ContainsWarehouseCatalogKey();
        }

        /// <summary>
        /// Gets the total price of an Item Cache (basket or wish list).
        /// </summary>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        /// <typeparam name="TLineItemModel">
        /// The type of the line item
        /// </typeparam>
        /// <returns>
        /// The total price.
        /// </returns>
        public static decimal Total<TLineItemModel>(this IItemCacheModel<TLineItemModel> itemCache)
            where TLineItemModel : class, ILineItemModel, new()
        {
            var subTotal = itemCache.Items.Where(x => x.LineItemType != LineItemType.Discount).Sum(x => x.Total());
            var discounts = itemCache.Items.Where(x => x.LineItemType == LineItemType.Discount).Sum(x => x.Total());

            return subTotal - discounts;
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

        /// <summary>
        /// Gets a value indicating whether the summary contains shippable items.
        /// </summary>
        /// <param name="summary">
        /// The summary.
        /// </param>
        /// <typeparam name="TBillingAddress">
        /// The type of the billing address
        /// </typeparam>
        /// <typeparam name="TShippingAddress">
        /// The type of the shipping address
        /// </typeparam>
        /// <typeparam name="TLineItem">
        /// The type of the line item
        /// </typeparam>
        /// <returns>
        /// The value indicating whether the summary contains shippable items.
        /// </returns>
        public static bool HasShippableItems<TBillingAddress, TShippingAddress, TLineItem>(
            this ICheckoutSummaryModel<TBillingAddress, TShippingAddress, TLineItem> summary)
             where TBillingAddress : class, ICheckoutAddressModel, new()
        where TShippingAddress : class, ICheckoutAddressModel, new()
        where TLineItem : class, ILineItemModel, new()
        {
            return summary.Items.Any(x => x.IsShippable());
        }
    }
}