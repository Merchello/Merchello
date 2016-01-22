namespace Merchello.Web.Models.ContentEditing
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Models;

    /// <summary>
    /// Represents a customer item cache.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here.")]
    public class CustomerItemCacheDisplay : LineItemDisplayCollectionBase<ItemCacheLineItemDisplay>
    {
        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        public CustomerDisplay Customer { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public override IEnumerable<ItemCacheLineItemDisplay> Items { get; set; }
    }

    /// <summary>
    /// The basket display extension.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class CustomerItemCacheDisplayExtension
    {
        /// <summary>
        /// Maps a customer item cache (basket) to <see cref="CustomerItemCacheDisplay"/>
        /// </summary>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerItemCacheDisplay"/>.
        /// </returns>
        public static CustomerItemCacheDisplay ToCustomerItemCacheDisplay(this IItemCache itemCache)
        {
            return AutoMapper.Mapper.Map<CustomerItemCacheDisplay>(itemCache);
        }
    }
}