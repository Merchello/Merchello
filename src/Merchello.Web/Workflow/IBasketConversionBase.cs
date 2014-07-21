namespace Merchello.Web.Workflow
{
    using Merchello.Core.Strategies.Merging;

    /// <summary>
    /// Marker interface for the basket conversion strategy.
    /// </summary>
    public interface IBasketConversionBase : ILineItemContainerMergingStrategy<IBasket>
    {
    }
}