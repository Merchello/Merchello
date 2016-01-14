namespace Merchello.Web.Workflow
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Checkout;
    using Merchello.Core.Models;
    using Merchello.Web.Workflow.Checkout;

    using Umbraco.Core.Logging;

    /// <summary>
    /// Extension methods for the <see cref="IBasket"/> and <see cref="IWishList"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed. Suppression is OK here.")]
    public static class BasketWishListExtensions
    {

        /// <summary>
        /// Saves the entire basket to the wish list and then clears the basket.
        /// </summary>
        /// <param name="basket">
        /// The <see cref="IBasket"/>.
        /// </param>
        public static void SaveToWishList(this IBasket basket)
        {
            // Anonymous customers do not have wish lists
            if (basket.Customer.IsAnonymous)
            {
                LogHelper.Debug(typeof(BasketWishListExtensions), "SaveToWishList called on an anonymous customer basket.");
                return;
            }

            var customer = (ICustomer)basket.Customer;

            customer.WishList().Items.Add(basket.Items.Select(x => x.AsLineItemOf<ItemCacheLineItem>()));
            customer.WishList().Save();

            basket.Empty();
        }

        /// <summary>
        /// Moves a item to the wish list.
        /// </summary>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        public static void MoveItemToWishList(this IBasket basket, string sku)
        {
            var lineItem = basket.Items.FirstOrDefault(x => x.Sku.Equals(sku));
            basket.MoveItemToWishList(lineItem);
        }

        /// <summary>
        /// Moves a item to the wish list.
        /// </summary>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <param name="lineItemKey">
        /// The line item key.
        /// </param>
        public static void MoveItemToWishList(this IBasket basket, Guid lineItemKey)
        {
            // Anonymous customers do not have wish lists
            if (basket.Customer.IsAnonymous)
            {
                LogHelper.Debug(typeof(BasketWishListExtensions), "SaveToWishList called on an anonymous customer basket.");
                return;
            }

            var lineItem = basket.Items.FirstOrDefault(x => x.Key == lineItemKey);
            
            if (lineItem == null) return;
            
            var wishList = ((ICustomer)basket.Customer).WishList();
            wishList.AddItem(lineItem.AsLineItemOf<ItemCacheLineItem>());
            wishList.Save();

            basket.RemoveItem(lineItem.Key);
            basket.Save();
        }

        /// <summary>
        /// Moves a item to the wish list.
        /// </summary>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        public static void MoveItemToWishList(this IBasket basket, ILineItem item)
        {
            basket.MoveItemToWishList(item.Key);
        }

        /// <summary>
        /// Moves the wish list to the basket.
        /// </summary>
        /// <param name="wishList">
        /// The wish list.
        /// </param>
        public static void MoveToBasket(this IWishList wishList)
        {
            var basket = wishList.Customer.Basket();

            basket.Items.Add(wishList.Items.Select(x => x.AsLineItemOf<ItemCacheLineItem>()));
            basket.Save();

            wishList.Empty();
        }

        /// <summary>
        /// Moves the wish list to the basket.
        /// </summary>
        /// <param name="wishList">
        /// The wish list.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        public static void MoveItemToBasket(this IWishList wishList, string sku)
        {
            var lineItem = wishList.Items.FirstOrDefault(x => x.Sku.Equals(sku));
            wishList.MoveItemToBasket(lineItem);
        }

        /// <summary>
        /// Moves the wish list to the basket.
        /// </summary>
        /// <param name="wishList">
        /// The wish list.
        /// </param>
        /// <param name="lineItemKey">
        /// The line Item Key.
        /// </param>
        public static void MoveItemToBasket(this IWishList wishList, Guid lineItemKey)
        {
            var lineItem = wishList.Items.FirstOrDefault(x => x.Key == lineItemKey);

            if (lineItem == null) return;

            var basket = wishList.Customer.Basket();
            basket.AddItem(lineItem.AsLineItemOf<ItemCacheLineItem>());
            basket.Save();

            wishList.RemoveItem(lineItem.Key);
            wishList.Save();
        }

        /// <summary>
        /// Moves the wish list to the basket.
        /// </summary>
        /// <param name="wishList">
        /// The wish list.
        /// </param>
        /// <param name="lineItem">
        /// The line Item.
        /// </param>
        public static void MoveItemToBasket(this IWishList wishList, ILineItem lineItem)
        {
            wishList.MoveItemToBasket(lineItem.Key);
        }
    }
}