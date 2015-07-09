namespace Merchello.Web.Workflow
{
    using System;
    using Core.Models;

    using Merchello.Web.Workflow.CustomerItemCache;

    /// <summary>
    /// Defines a wish list.
    /// </summary>
    public interface IWishList : ICustomerItemCacheBase
    {
        /// <summary>
        /// Gets the sum of all wish list item "amount" multiplied by quantity (price)
        /// </summary>
        decimal TotalWishListPrice { get; }        
    }
}