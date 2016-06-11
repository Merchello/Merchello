namespace Merchello.Core.Checkout
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;

    /// <summary>
    /// Extension methods for the CheckoutManager.
    /// </summary>
    internal static class CheckoutManagerExtensions
    {
        /// <summary>
        /// Gets a clone of the ItemCache
        /// </summary>
        /// <param name="checkoutManager">
        /// The <see cref="ICheckoutManagerBase"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IItemCache"/>.
        /// </returns>
        internal static IItemCache CloneItemCache(this ICheckoutManagerBase checkoutManager)
        {
            // The ItemCache needs to be cloned as line items may be altered while applying constraints
            return CloneItemCache(checkoutManager.Context.ItemCache);
        }

        /// <summary>
        /// Clones a <see cref="ILineItemContainer"/> as <see cref="IItemCache"/>
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="IItemCache"/>.
        /// </returns>
        internal static IItemCache CloneItemCache(ILineItemContainer container)
        {
            var clone = new ItemCache(Guid.NewGuid(), ItemCacheType.Backoffice);
            foreach (var item in container.Items)
            {
                clone.Items.Add(item.AsLineItemOf<ItemCacheLineItem>());
            }

            return clone;
        }

        /// <summary>
        /// Creates a new <see cref="ILineItemContainer"/> with filtered items.
        /// </summary>
        /// <param name="filteredItems">
        /// The line items.
        /// </param>
        /// <returns>
        /// The <see cref="ILineItemContainer"/>.
        /// </returns>
        internal static ILineItemContainer CreateNewLineContainer(IEnumerable<ILineItem> filteredItems)
        {
            return LineItemExtensions.CreateNewItemCacheLineItemContainer(filteredItems);
        }
    }
}