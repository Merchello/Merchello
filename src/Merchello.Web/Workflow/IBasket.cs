namespace Merchello.Web.Workflow
{
    using Merchello.Web.Workflow.CustomerItemCache;

    /// <summary>
    /// The Basket interface.
    /// </summary>
    public interface IBasket : ICustomerItemCacheBase
    {        
        /// <summary>
        /// Gets the sum of all basket item "amount" multiplied by quantity (price)
        /// </summary>
        decimal TotalBasketPrice { get; }       
    }
}