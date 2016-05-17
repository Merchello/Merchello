namespace Merchello.Web.Models.ContentEditing
{
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Models;

    /// <summary>
    /// The item cache line item display.
    /// </summary>
    public class ItemCacheLineItemDisplay : LineItemDisplayBase
    {
    }

    /// <summary>
    /// The item cache line item display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class ItemCacheLineItemDisplayExtensions
    {
        /// <summary>
        /// Maps <see cref="IItemCacheLineItem"/> to <see cref="ItemCacheLineItemDisplay"/>.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <returns>
        /// The <see cref="ItemCacheLineItemDisplay"/>.
        /// </returns>
        public static ItemCacheLineItemDisplay ToItemCacheLineItemDisplay(this IItemCacheLineItem lineItem)
        {
            return AutoMapper.Mapper.Map<IItemCacheLineItem, ItemCacheLineItemDisplay>(lineItem);
        }
    }
}