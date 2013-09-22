namespace Merchello.Core.OrderFulfillment.Strategies
{
    /// <summary>
    /// Defines the fulfillment strategy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFulfillmentStrategy : IOrderFulfillment
    {
        void Process();
    }
}
