namespace Merchello.Implementation
{
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Newtonsoft.Json;

    /// <summary>
    /// Extension methods for <see cref="ItemCacheLineItem"/> (Merchello Core Basket and Wish List Items).
    /// </summary>
    public static class ItemCacheLineItemExtensions
    {
        /// <summary>
        /// Gets the option choice pairs for a variant added as a basket item.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <returns>
        /// The option choice pairs for a variant added to the basket.
        /// </returns>
        public static Dictionary<string, string> GetProductOptionChoicePairs(this ILineItem lineItem)
        {
            return
                lineItem.ExtendedData.GetValue<Dictionary<string, string>>(
                    Constants.ExtendedDataKeys.BasketItemCustomerChoice);
        } 
    }
}